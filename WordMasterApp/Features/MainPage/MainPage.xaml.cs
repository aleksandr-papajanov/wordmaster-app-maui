using ReactiveUI;
using System.Reactive.Disposables;
using WordMasterApp.Features.DeckList;

namespace WordMasterApp.Features.MainPage;

public partial class MainPage : ContentPage, IViewFor<MainViewModel>
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(MainViewModel), typeof(MainPage),
            propertyChanged: (bindable, _, value) =>
            {
            });

    // Implementation of IViewFor<>.ViewModel
    public MainViewModel? ViewModel
    {
        get => BindingContext as MainViewModel ?? null;
        set => BindingContext = value;
    }

    // Explicit implementation of IViewFor.ViewModel
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as MainViewModel;
    }


    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm, view => view.BindingContext)
                .DisposeWith(disposables);
        });
    }
}
