using DynamicData;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace WordMasterApp.Components.BlobCollection
{
    public class BlobCollectionViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly ISubject<IObservable<IChangeSet<IBlobCollectionDisplayable>>> _stream;
        private ReadOnlyObservableCollection<BlobCollectionItem<IBlobCollectionDisplayable>> _items = null!;
        public ReadOnlyObservableCollection<BlobCollectionItem<IBlobCollectionDisplayable>> Items => _items;

        private Guid? _selectedItemId;
        public Guid? SelectedItemId
        {
            get => _selectedItemId;
            set => this.RaiseAndSetIfChanged(ref _selectedItemId, value);
        }

        public ICommand SelectCommand { get; }

        public BlobCollectionViewModel(ISubject<IObservable<IChangeSet<IBlobCollectionDisplayable>>> stream)
        {
            _stream = stream;
            SelectCommand = ReactiveCommand.Create<BlobCollectionItem<IBlobCollectionDisplayable>>(SetSelected);

            this.WhenActivated(disposables =>
            {
                _stream
                    .Switch()
                    .Transform(item =>
                    {
                        var blob = new BlobCollectionItem<IBlobCollectionDisplayable>(item);
                        blob.IsSelected = item.Id == SelectedItemId;
                        return blob;
                    })
                    .AutoRefreshOnObservable(_ => this.WhenAnyValue(vm => vm.SelectedItemId))
                    .Do(changeSet =>
                    {
                        foreach (var item in _items)
                        {
                            item.IsSelected = item.OriginalItem.Id == SelectedItemId;
                        }
                    })
                    .Bind(out _items)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .DisposeMany()
                    .Subscribe(_ =>
                    {
                        this.RaisePropertyChanged(nameof(Items));
                    })
                    .DisposeWith(disposables);
            });
        }

        public void SetSelected(BlobCollectionItem<IBlobCollectionDisplayable> selected)
        {
            SelectedItemId = selected.Id;
        }


        // Implementing IActivatableViewModel to support activation and deactivation of the view model
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
