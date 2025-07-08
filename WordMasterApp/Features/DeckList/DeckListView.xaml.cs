
using ReactiveUI;
using System.Reactive.Disposables;

namespace WordMasterApp.Features.DeckList;

public partial class DeckListView : ContentView, IViewFor<DeckListViewModel>
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(DeckListViewModel), typeof(DeckListView),
            propertyChanged: (bindable, _, value) =>
            {
                if (bindable is not DeckListView view || value is not DeckListViewModel viewModel)
                    return;

                view.ViewModel = viewModel;
                view.BindingContext = viewModel;
            });

    public DeckListViewModel? ViewModel
    {
        get => (DeckListViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as DeckListViewModel;
    }


    public DeckListView(DeckListViewModel viewModel)
    {
        ViewModel = viewModel;
        BindingContext = viewModel;

        InitializeComponent();

        this.WhenAnyValue(x => x.ViewModel)
            .WhereNotNull()
            .Subscribe(vm =>
            {
                this.WhenActivated(disposables =>
                {
                    // Bindings
                    this.OneWayBind(ViewModel, vm => vm.Decks, view => view.Decks.ItemsSource)
                        .DisposeWith(disposables);
                });
            });
    }
}