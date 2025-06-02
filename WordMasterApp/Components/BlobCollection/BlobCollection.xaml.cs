using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Maui;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using WordMaster.Data.DTOs;

namespace WordMasterApp.Components;

public partial class BlobCollection : ContentView, IViewFor<BlobCollectionViewModel>
{
    private readonly CompositeDisposable _disposables = new();

    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(BlobCollectionViewModel), typeof(BlobCollection));

    public BlobCollectionViewModel ViewModel
    {
        get => (BlobCollectionViewModel)GetValue(ViewModelProperty);
        set
        {
            SetValue(ViewModelProperty, value);
            OnViewModelSet();
        }
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (BlobCollectionViewModel)value!;
    }

    private ReadOnlyObservableCollection<BlobCollectionItem<IDisblayable>> _blobItems = null!;
    public ReadOnlyObservableCollection<BlobCollectionItem<IDisblayable>> BlobItems => _blobItems;

    public ICommand SelectCommand { get; }

    public BlobCollection()
    {
        InitializeComponent();
        SelectCommand = ReactiveCommand.Create<BlobCollectionItem<IDisblayable>>(OnSelect);
    }

    public void OnViewModelSet()
    {
        _disposables.Clear();

        if (ViewModel == null)
            return;

        ViewModel
            .Connect()
            .Transform(item =>
            {
                var blob = new BlobCollectionItem<IDisblayable>(item);
                blob.IsSelected = item.Id == ViewModel.SelectedItemId;
                return blob;
            })
            .AutoRefreshOnObservable(_ => ViewModel.WhenAnyValue(vm => vm.SelectedItemId))
            .Do(changeSet =>
            {
                foreach (var item in _blobItems)
                {
                    item.IsSelected = item.OriginalItem.Id == ViewModel.SelectedItemId;
                }
            })
            .Bind(out _blobItems)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ =>
            {
                this.OnPropertyChanged(nameof(BlobItems));
            })
            .DisposeWith(_disposables);
    }

    private void OnSelect(BlobCollectionItem<IDisblayable> item)
    {
        ViewModel!.SelectedItemId = item.Id;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        _disposables.Clear();
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();
        if (Parent == null)
        {
            _disposables.Dispose();
        }
    }
}
