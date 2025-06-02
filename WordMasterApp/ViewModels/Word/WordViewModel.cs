using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using WordMaster.Data.DTOs;
using WordMaster.Data.Infrastructure.Interfaces;
using WordMasterApp.Components;

namespace WordMasterApp.ViewModels.Word
{
    public partial class WordViewModel : ReactiveObject, IDisposable
    {
        private readonly IWordService _wordService;
        private readonly CompositeDisposable _disposables = new();
        private readonly SourceList<IDisblayable> _staticItems = new();

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        private WordDTO? _selectedWord;
        public WordDTO? SelectedWord
        {
            get => _selectedWord;
            set => this.RaiseAndSetIfChanged(ref _selectedWord, value);
        }

        private readonly ObservableAsPropertyHelper<int> _foundCount;
        public int FoundCount => _foundCount.Value;

        public ReactiveCommand<Unit, Unit> UpdateCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

        public BlobCollectionViewModel WordBlobsViewModel { get; } = new();

        public WordViewModel(IWordService wordService)
        {
            _wordService = wordService;

            // Add static placeholder item
            _staticItems.Add(new NewWordPlaceholder());

            // Setup commands
            var canUpdate = WordBlobsViewModel.WhenAnyValue(vm => vm.SelectedItemId, (Guid? id) => id != null);
            UpdateCommand = ReactiveCommand.CreateFromTask(UpdateWordAsync, canUpdate);

            var canDelete = WordBlobsViewModel.WhenAnyValue(vm => vm.SelectedItemId, (Guid? id) => id != null);
            DeleteCommand = ReactiveCommand.CreateFromTask(DeleteWordAsync, canDelete);

            // Setup main collection pipeline
            _wordService.WordsSource
                .Connect()
                .Transform(word => word as IDisblayable)
                .MergeChangeSets(_staticItems.Connect())
                .Sort(SortExpressionComparer<IDisblayable>
                    .Ascending(x => x is NewWordPlaceholder ? 0 : 1))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var items)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(_disposables);

            WordBlobsViewModel.Items = items;

            // Keep FoundCount in sync (excluding placeholder)
            WordBlobsViewModel
                .Connect()
                .ToCollection()
                .Select(list => list.Count(x => x is not NewWordPlaceholder))
                .ToProperty(this, x => x.FoundCount, out _foundCount)
                .DisposeWith(_disposables);

            // Search text filter
            this.WhenAnyValue(vm => vm.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Select(text => text?.Trim() ?? string.Empty)
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(filterString => _wordService.FilterSubject.OnNext(filterString))
                .DisposeWith(_disposables);

            // Selection logic
            WordBlobsViewModel
                .WhenAnyValue(vm => vm.SelectedItemId)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(id =>
                {
                    var found = items.FirstOrDefault(w => w.Id == id);
                    SelectedWord = found is NewWordPlaceholder
                        ? new WordDTO()
                        : found as WordDTO;
                })
                .DisposeWith(_disposables);
        }


        private async Task UpdateWordAsync()
        {
            if (SelectedWord == null)
                return;

            if (SelectedWord.Id == Guid.Empty)
            {
                await _wordService.CreateAsync(SelectedWord);
                SelectedWord = new WordDTO();
            }
            else
            {
                await _wordService.UpdateAsync(SelectedWord);
            }
        }

        private async Task DeleteWordAsync()
        {
            if (SelectedWord == null)
                return;

            await _wordService.DeleteAsync(SelectedWord.Id);
            WordBlobsViewModel.SelectedItemId = null;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}

