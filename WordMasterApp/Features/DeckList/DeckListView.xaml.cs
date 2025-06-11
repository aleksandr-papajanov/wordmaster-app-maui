
using ReactiveUI;
using System.Reactive.Disposables;

namespace WordMasterApp.Features.DeckList;

public partial class DeckListView : ContentView, IViewFor<DeckListViewModel>
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(DeckListViewModel), typeof(DeckListView),
            propertyChanged: (bindable, _, value) =>
            {
            });

    // Implementation of IViewFor<>.ViewModel
    public DeckListViewModel? ViewModel
    {
        get => BindingContext as DeckListViewModel ?? null;
        set => BindingContext = value;
    }

    // Explicit implementation of IViewFor.ViewModel
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as DeckListViewModel;
    }


    public DeckListView(DeckListViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        
        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm, view => view.BindingContext)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.Decks, view => view.Decks.ItemsSource)
                .DisposeWith(disposables);
        });
    }
}