using DynamicData;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using WordMasterApp.EntityViewModels;
using WordMasterApp.Features.MessageContainer;
using WordMasterApp.Features.WordUsage;
using WordMasterApp.Messages;

namespace WordMasterApp.Features
{
    public partial class WordDetailsViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly IMessageService _messageService;
        private readonly IWordUsageViewViewModelFactory _wordUsageFactory;

        private readonly ObservableAsPropertyHelper<WordEntityViewModel?> _currentWord;
        public WordEntityViewModel? CurrentWord => _currentWord.Value;

        private ReadOnlyObservableCollection<RelatedWordEntityViewModel> _synonyms = null!;
        public ReadOnlyObservableCollection<RelatedWordEntityViewModel> Synonyms => _synonyms;

        private ReadOnlyObservableCollection<RelatedWordEntityViewModel> _antonyms = null!;
        public ReadOnlyObservableCollection<RelatedWordEntityViewModel> Antonyms => _antonyms;

        private bool _hasTriedToUpdate = false;
        public bool HasTriedToUpdate
        {
            get => _hasTriedToUpdate;
            private set => this.RaiseAndSetIfChanged(ref _hasTriedToUpdate, value);
        }

        // ViewModels
        public WordUsageViewViewModel WordUsageViewModel { get; }

        // Commands
        public ICommand UpdateCommand { get; private set; }
        public ICommand AutoCompleteCommand { get; private set; }
        public ICommand ExplainRelatedWordCommand { get; private set; }


        // Implementing IActivatableViewModel
        public ViewModelActivator Activator { get; } = new ViewModelActivator();


        public WordDetailsViewModel(IObservable<WordEntityViewModel?> word,
                                    IMessageService messageService,
                                    IWordUsageViewViewModelFactory wordUsageFactory)
        {
            _messageService = messageService;
            _wordUsageFactory = wordUsageFactory;

            word.ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.CurrentWord, out _currentWord);

            //word.ObserveOn(RxApp.MainThreadScheduler)
            //    .Subscribe(x => CurrentWord = x);

            SetupCommands();

            // important to create the view models in constructor for propper binding
            WordUsageViewModel = wordUsageFactory.Create(word);

            this.WhenActivated(disposables =>
            {
                word.Subscribe(word =>
                {
                    HasTriedToUpdate = false;
                })
                .DisposeWith(disposables);

                this.WhenAnyValue(x => x.CurrentWord)
                    .Select(x => x != null && x.IsManaged
                        ? x.Synonyms
                        : Observable.Return(ChangeSet<RelatedWordEntityViewModel>.Empty))
                    .Switch()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out _synonyms)
                    .Subscribe(_ => this.RaisePropertyChanged(nameof(Synonyms)))
                    .DisposeWith(disposables);
                
                this.WhenAnyValue(x => x.CurrentWord)
                    .Select(x => x != null && x.IsManaged
                        ? x.Antonyms
                        : Observable.Return(ChangeSet<RelatedWordEntityViewModel>.Empty))
                    .Switch()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out _antonyms)
                    .Subscribe(_ => this.RaisePropertyChanged(nameof(Antonyms)))
                    .DisposeWith(disposables);
            });
        }


        private void SetupCommands()
        {
            var word  = this.WhenAnyValue(x => x.CurrentWord).Select(x => x != null);
            var isNew = this.WhenAnyValue(x => x.CurrentWord).Select(x => !(x == null || x.IsManaged));
            var tried = this.WhenAnyValue(x => x.HasTriedToUpdate);
            var valid = this.WhenAnyValue(x => x.CurrentWord).Select(x => x != null ? x.IsValidChanged : Observable.Return(false)).Switch();

            var canUpdate = Observable.CombineLatest(word, tried, valid, (word, tried, valid) => word && (tried ? valid : true));
            //var canComplete = Observable.CombineLatest(managed, (managed) => !managed);

            UpdateCommand = ReactiveCommand.CreateFromTask(UpdateWordAsync, canUpdate);
            AutoCompleteCommand = ReactiveCommand.CreateFromTask(AutoCompleteAsync, isNew);
            ExplainRelatedWordCommand = ReactiveCommand.CreateFromTask<RelatedWordEntityViewModel>(ExplainRelatedWordAsync, word);
        }

        private async Task ExplainRelatedWordAsync(RelatedWordEntityViewModel relatedWord)
        {
            //if (relatedWord == null || CurrentWord == null || relatedWord.WordId != CurrentWord.Id)
            //    return;

            //var session = _wordModuleService.Generations.CompleteWordBase(new WordCompletionRequest(
            //    Word: relatedWord.Text,
            //    SourceLanguage: CurrentWord?.Deck?.SourceLanguage?.Name ?? throw new Exception("Source language is not set for the current deck."),
            //    TargetLanguage: CurrentWord?.Deck?.TargetLanguage?.Name ?? throw new Exception("Target language is not set for the current deck.")
            //));

            //session.OnComplete += async (context) =>
            //{
            //    if (context.SessionResult == GenerationSessionResult.Error)
            //    {
            //        _messageService.Publish(new ErrorMessage(context.ErrorMessage ?? "An error occurred during word completion."));
            //    }
            //    else if (context.SessionResult == GenerationSessionResult.Success)
            //    {
            //        var text = context.Get<string>(WordBaseCompletionKeys.Word);
            //        var trans = context.Get<string>(WordBaseCompletionKeys.Translation);
            //        var def = context.Get<string>(WordBaseCompletionKeys.Definition);
            //        var freq = context.Get<WordUsageFrequenсy>(WordBaseCompletionKeys.UsageFrequency);

            //        var box = new ConfirmWordCompletionMessage(text, trans, def, freq);

                    

            //        _messageService.Publish(box);
            //    }

            //    await Task.CompletedTask;
            //};

            //await session.RunAsync();
        }

        private async Task AutoCompleteAsync()
        {
            if (CurrentWord == null)
                return;

            await CurrentWord.AutoCompleteAsync((message) => _messageService.Publish(message));
        }

        private async Task UpdateWordAsync()
        {
            if (CurrentWord == null)
                return;

            if (CurrentWord.IsValid)
            {
                await CurrentWord.SaveAsync();

                if (CurrentWord.IsManaged)
                {
                    MessageBus.Current.SendMessage(new WordCreatedMessage(CurrentWord.Id));

                    HasTriedToUpdate = false;
                }
            }
            else
            {
                HasTriedToUpdate = true;
            }
        }
    }
}