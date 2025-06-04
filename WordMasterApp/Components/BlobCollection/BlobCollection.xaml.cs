using ReactiveUI;
using System.Reactive.Disposables;

namespace WordMasterApp.Components.BlobCollection;

public partial class BlobCollection : ContentView, IViewFor<BlobCollectionViewModel>
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(BlobCollectionViewModel), typeof(BlobCollection),
            propertyChanged: (bindable, _, value) =>
            {
                if (bindable is not BlobCollection view || value is not BlobCollectionViewModel viewModel)
                {
                    return;
                }

                view.BindingContext = viewModel;
                view.WhenActivated(disposables =>
                {
                    //view.BindCommand(viewModel, vm => vm.SelectCommand, v => v)
                    //    .DisposeWith(disposables);
                });
            });

    // Implementation of IViewFor<WordUsageViewModel>.ViewModel
    public BlobCollectionViewModel? ViewModel
    {
        get => BindingContext as BlobCollectionViewModel ?? null;
        set => BindingContext = value;
    }

    // Explicit implementation of IViewFor.ViewModel
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as BlobCollectionViewModel;
    }

    public BlobCollection()
    {
        InitializeComponent();
    }
}
