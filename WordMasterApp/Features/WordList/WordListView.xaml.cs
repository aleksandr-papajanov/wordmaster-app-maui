using ReactiveUI;
using System.Reactive.Disposables;

namespace WordMasterApp.Features.WordList;

public partial class WordListView : ContentView, IViewFor<WordListViewViewModel>
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(WordListViewViewModel), typeof(WordListView),
            propertyChanged: (bindable, _, value) =>
            {
                if (bindable is not WordListView view || value is not WordListViewViewModel viewModel)
                    return;

                view.ViewModel = viewModel;
                view.BindingContext = viewModel;
            });

    public WordListViewViewModel? ViewModel
    {
        get => (WordListViewViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as WordListViewViewModel;
    }

    public WordListView(WordListViewViewModel viewModel)
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
                });
            });
    }
}