using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WordMaster.Data.Services;
using WordMaster.Data.ViewModels;
using WordMasterApp.Components.BlobCollection;
using WordMasterApp.Infrastructure;

namespace WordMasterApp.Features
{
    public partial class WordDetailsViewModel : ReactiveObject, IActivatableViewModel, IValidatableViewModel
    {
        private readonly IWordService _wordService;
        private readonly IWordViewModelFactory _wordViewModelFactory;
        private readonly SourceList<IBlobCollectionDisplayable> _staticItems = new();
        private readonly ReplaySubject<IObservable<IChangeSet<IBlobCollectionDisplayable>>> _wordsSubject = new();

        // In UI determines whether we show validation errors or not
        private bool _hasTriedToUpdate = false;
        public bool HasTriedToUpdate
        {
            get => _hasTriedToUpdate;
            private set => this.RaiseAndSetIfChanged(ref _hasTriedToUpdate, value);
        }

        // Binded to the UI entry fields
        private WordViewModel? _selectedWord;
        public WordViewModel? SelectedWord
        {
            get => _selectedWord;
            private set => this.RaiseAndSetIfChanged(ref _selectedWord, value);
        }

        // Search text binded to the UI entry field
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }


        private ObservableAsPropertyHelper<int> _foundCount;
        public int FoundCount => _foundCount?.Value ?? 0;

       
        // ViewModels for Blobs and Usage
        public BlobCollectionViewModel WordBlobsVM { get; }
        public WordUsageViewModel WordUsageVM { get; }

        // Commands
        public ReactiveCommand<Unit, Unit> UpdateCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }



        public WordDetailsViewModel(IWordService wordService,
                                    IBlobCollectionViewModelFactory blobCollectionViewModelFactory,
                                    IWordUsageViewModelFactory wordUsageFactory,
                                    IWordViewModelFactory wordViewModelFactory)
        {
            _wordService = wordService;
            _wordViewModelFactory = wordViewModelFactory;

            SetupValidation();

            // important to create the view models in constructor for propper binding
            WordBlobsVM = blobCollectionViewModelFactory.Create(_wordsSubject);
            WordUsageVM = wordUsageFactory.Create(
                this.WhenAnyValue(vm => vm.SelectedWord)
                    .WhereNotNull());

            // Add static placeholder item
            _staticItems.Add(new NewWordPlaceholder());

            // Setup commands
            var isTried = this.WhenAnyValue(x => x.HasTriedToUpdate);
            var isValid = this.ValidationContext.WhenAnyValue(x => x.IsValid);
            var isSelected = WordBlobsVM
                .WhenAnyValue(vm => vm.SelectedItemId)
                .Select(id => id != null);

            var canUpdate = Observable.CombineLatest(
                isSelected, isValid, isTried,
                (selected, valid, tried) => selected && (tried ? valid : true));

            UpdateCommand = ReactiveCommand.CreateFromTask(UpdateWordAsync, canUpdate);
            DeleteCommand = ReactiveCommand.CreateFromTask(DeleteWordAsync, isSelected);


            this.WhenActivated(disposables =>
            {
                // Setup main collection pipeline
                _wordService.Words
                    .Transform(word => _wordViewModelFactory.Create(word) as IBlobCollectionDisplayable)
                    .MergeChangeSets(_staticItems.Connect())
                    .Sort(SortExpressionComparer<IBlobCollectionDisplayable>
                        .Ascending(x => x is NewWordPlaceholder ? 0 : 1))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out var items)
                    .DisposeMany()
                    .Subscribe()
                    .DisposeWith(disposables);

                // Notify the viewmodel about the changes in the collection
                _wordsSubject.OnNext(items.ToObservableChangeSet());

                // Keep track of the found count
                items.WhenAnyValue(items => items.Count)
                    .Select(count => count - _staticItems.Count) // don't cound static placeholders
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .ToProperty(this, x => x.FoundCount, out _foundCount)
                    .DisposeWith(disposables);

                // Search text filter
                this.WhenAnyValue(x => x.SearchText)
                    .Throttle(TimeSpan.FromMilliseconds(200))
                    .Select(text => text?.Trim() ?? string.Empty)
                    .DistinctUntilChanged()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(text => _wordService.FilterSubject.OnNext(text))
                    .DisposeWith(disposables);

                // Bind selected word based on the selected item in the collection so that UI updates accordingly
                WordBlobsVM
                    .WhenAnyValue(x => x.SelectedItemId)
                    .Select(id =>
                    {
                        var found = items.FirstOrDefault(w => w.Id == id);
                        return found is NewWordPlaceholder
                            ? _wordViewModelFactory.Create()
                            : found as WordViewModel;
                    })
                    .Do(_ => HasTriedToUpdate = false) // Reset the update flag when a new word is selected
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, x => x.SelectedWord)
                    .DisposeWith(disposables);
            });
        }

        private void SetupValidation()
        {
            this.ValidationRule(
                vm => vm.SelectedWord.Text,
                text => !string.IsNullOrWhiteSpace(text),
                "Word cannot be empty"
            );

            this.ValidationRule(
                vm => vm.SelectedWord.Translation,
                text => !string.IsNullOrWhiteSpace(text),
                "Translation cannot be empty"
            );
        }


        private async Task UpdateWordAsync()
        {
            if (SelectedWord == null)
                return;

            if (ValidationContext.IsValid)
            {
                await SelectedWord.UpdateAsync();
                WordBlobsVM.SelectedItemId = SelectedWord.Id; // Update selection in UI
                HasTriedToUpdate = false;
            }
            else
            {
                HasTriedToUpdate = true;
            }
        }

        private async Task DeleteWordAsync()
        {
            if (SelectedWord == null)
                return;

            WordBlobsVM.SelectedItemId = null;
            await SelectedWord.DeleteAsync();
        }


        // Implementing IValidatableViewModel requires a ValidationContext property
        public IValidationContext ValidationContext { get; } = new ValidationContext();

        // Implementing IActivatableViewModel requires an Activator property
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}