using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using WordMaster.Data.Models;
using WordMaster.Data.Services.Interfaces;
using WordMaster.Data.ViewModels;
using WordMasterApp.DIFactories;
using WordMasterApp.EntityWrappers;
using WordMasterApp.Messages;


namespace WordMasterApp.Features.WordList
{
    public class WordListViewViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly IWordService _wordService;
        private readonly IWordWrapperViewModelDIFactory _wordWrapperFactory;

        private readonly BehaviorSubject<string> _searchTextSubject = new(string.Empty);
        private readonly SourceList<WordWrapperViewModel> _staticItems = new();

        private readonly ObservableAsPropertyHelper<DeckWrapperViewModel?> _currentDeck;
        public DeckWrapperViewModel? CurrentDeck => _currentDeck.Value;

        private ReadOnlyObservableCollection<WordWrapperViewModel> _words = null!;
        public ReadOnlyObservableCollection<WordWrapperViewModel> Words => _words;

        private WordWrapperViewModel? _selectedWord;
        public WordWrapperViewModel? SelectedWord
        {
            get => _selectedWord;
            set => this.RaiseAndSetIfChanged(ref _selectedWord, value);
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        // Is needed for "outside world" to observe changes
        public IObservable<WordWrapperViewModel?> SelectedWordObservable
            => this.WhenAnyValue(vm => vm.SelectedWord);
                   //.Select(word => word != null && word.IsManaged ? word : null);

        public ICommand SelectWordCommand { get; private set; }
        public ICommand CreateWordCommand { get; private set; }
        public ICommand DeleteWordCommand { get; private set; }

        // Implementing IActivatableViewModel requires an Activator property
        public ViewModelActivator Activator { get; } = new ViewModelActivator();


        public WordListViewViewModel(IObservable<DeckWrapperViewModel?> deck, IWordService wordService, IWordWrapperViewModelDIFactory wordWrapperFactory)
        {
            _wordService = wordService;
            _wordWrapperFactory = wordWrapperFactory;

            deck.ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.CurrentDeck, out _currentDeck);

            SetupCommands();

            this.WhenActivated(disposables =>
            {
                // Main pipeline
                Observable
                    .CombineLatest(deck, _searchTextSubject.AsObservable(), (deck, filter) => (deck, filter))
                    .Select(x => x.deck != null && x.deck.IsManaged
                        ? _wordService
                            .GetStream(x.deck.Id, x.filter)
                            .Transform(x => _wordWrapperFactory.Create(x))
                        : Observable.Return(ChangeSet<WordWrapperViewModel>.Empty))
                    .Switch()
                    .MergeChangeSets(_staticItems.Connect())
                        .Sort(SortExpressionComparer<WordWrapperViewModel>
                            .Ascending(x => !x.IsManaged ? 0 : 1))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out _words)
                    .DisposeMany()
                    .Subscribe(_ =>
                    {
                        // Ensure that the selected word is cleared if it no longer exists in the list
                        if (SelectedWord != null && !Words.Contains(SelectedWord))
                        {
                            //SelectedWord = null;
                        }

                        this.RaisePropertyChanged(nameof(Words));
                    })
                    .DisposeWith(disposables);

                // Handle search text changes
                this.WhenAnyValue(x => x.SearchText)
                    .Throttle(TimeSpan.FromMilliseconds(80))
                    .Select(text => text?.Trim() ?? string.Empty)
                    .DistinctUntilChanged()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(text => _searchTextSubject.OnNext(text))
                    .DisposeWith(disposables);

                MessageBus.Current
                    .Listen<WordCreatedMessage>()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(message =>
                    {
                        _staticItems.Clear();
                        SelectedWord = Words.FirstOrDefault(w => !_staticItems.Items.Contains(w) && w.Id == message.NewWordId);
                        
                    })
                    .DisposeWith(disposables);
            });
        }

        private void SetupCommands()
        {
            var deck = this.WhenAnyValue(x => x.CurrentDeck).Select(x => x != null);
            var word = this.WhenAnyValue(x => x.SelectedWord).Select(x => x != null);

            var canCreate = Observable.CombineLatest(deck, word, (deck, word) => deck);
            var canDelete = Observable.CombineLatest(deck, word, (deck, word) => word);

            SelectWordCommand = ReactiveCommand.Create<WordWrapperViewModel>(Select);
            CreateWordCommand = ReactiveCommand.Create(CreateWord, canCreate);
            DeleteWordCommand = ReactiveCommand.CreateFromTask(DeleteWord, canDelete);
        }

        private void Select(WordWrapperViewModel tapped)
        {
            if (SelectedWord != null && !SelectedWord.IsManaged)
            {
                _staticItems.Remove(SelectedWord);
            }

            SelectedWord = SelectedWord != tapped ? tapped : null;
        }

        private void CreateWord()
        {
            if (CurrentDeck == null)
                return;

            var newWord = _wordWrapperFactory.Create(CurrentDeck.Entity);
            _staticItems.Add(newWord);
            SelectedWord = newWord;
        }

        private async Task DeleteWord()
        {
            if (SelectedWord == null)
                return;

            await SelectedWord.DeleteAsync();
            SelectedWord = null;
        }
    }
}
