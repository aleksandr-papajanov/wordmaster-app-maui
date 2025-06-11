using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using System.Reactive.Disposables;

namespace WordMasterApp.Features.WordDetails;

public partial class WordDetailsView : ContentView, IViewFor<WordDetailsViewModel>
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(WordDetailsViewModel), typeof(WordDetailsView),
            propertyChanged: (bindable, _, value) =>
            {
            });

    // Implementation of IViewFor<>.ViewModel
    public WordDetailsViewModel? ViewModel
    {
        get => BindingContext as WordDetailsViewModel ?? null;
        set => BindingContext = value;
    }

    // Explicit implementation of IViewFor.ViewModel
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as WordDetailsViewModel;
    }


    public WordDetailsView(WordDetailsViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm, view => view.BindingContext)
                .DisposeWith(disposables);

            this.BindValidation(ViewModel, vm => vm.Text, v => v.TextValidationWrapper.ValidationMessage)
                .DisposeWith(disposables);

            this.BindValidation(ViewModel, vm => vm.Translation, v => v.TranslationValidationWrapper.ValidationMessage)
                .DisposeWith(disposables);
        });
    }
}