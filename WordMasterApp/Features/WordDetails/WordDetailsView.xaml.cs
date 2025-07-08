using ReactiveUI;
using System.Reactive.Disposables;

namespace WordMasterApp.Features.WordDetails;

public partial class WordDetailsView : ContentView, IViewFor<WordDetailsViewModel>
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(WordDetailsViewModel), typeof(WordDetailsView),
            propertyChanged: (bindable, _, value) =>
            {
                if (bindable is not WordDetailsView view || value is not WordDetailsViewModel viewModel)
                    return;

                view.ViewModel = viewModel;
                view.BindingContext = viewModel;
            });

    public WordDetailsViewModel? ViewModel
    {
        get => (WordDetailsViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as WordDetailsViewModel;
    }


    public WordDetailsView(WordDetailsViewModel viewModel)
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