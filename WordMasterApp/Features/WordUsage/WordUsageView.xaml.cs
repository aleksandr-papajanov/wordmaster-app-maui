using ReactiveUI;

namespace WordMasterApp.Features;

public partial class WordUsageView : ContentView, IViewFor<WordUsageViewModel>
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(WordUsageViewModel), typeof(WordUsageView),
            propertyChanged: (bindable, _, value) =>
            {
                if (bindable is not WordUsageView view || value is not WordUsageViewModel viewModel)
                {
                    return;
                }

                view.BindingContext = viewModel;
                view.WhenActivated(disposables =>
                {

                });
            });

    // Implementation of IViewFor<WordUsageViewModel>.ViewModel
    public WordUsageViewModel? ViewModel
    {
        get => BindingContext as WordUsageViewModel ?? null;
        set => BindingContext = value;
    }

    // Explicit implementation of IViewFor.ViewModel
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as WordUsageViewModel;
    }

    public WordUsageView()
    {
        InitializeComponent();
    }
}