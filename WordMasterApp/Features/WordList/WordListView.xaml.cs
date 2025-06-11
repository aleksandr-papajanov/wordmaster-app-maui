using ReactiveUI;
using System.Reactive.Disposables;

namespace WordMasterApp.Features.WordList;

public partial class WordListView : ContentView, IViewFor<WordListViewViewModel>
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(WordListViewViewModel), typeof(WordListView),
            propertyChanged: (bindable, _, value) =>
            {
            });

    // Implementation of IViewFor<>.ViewModel
    public WordListViewViewModel? ViewModel
    {
        get => BindingContext as WordListViewViewModel ?? null;
        set => BindingContext = value;
    }

    // Explicit implementation of IViewFor.ViewModel
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as WordListViewViewModel;
    }

    public WordListView(WordListViewViewModel viewModel)
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