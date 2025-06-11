using CommunityToolkit.Mvvm.Messaging;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using System.Reactive.Disposables;
using System.Windows.Input;
using WordMaster.Data.ViewModels;

namespace WordMasterApp.Features;

public partial class WordUsageView : ContentView, IViewFor<WordUsageViewViewModel>
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(WordUsageViewViewModel), typeof(WordUsageView),
            propertyChanged: (bindable, _, value) =>
            {
                if (bindable is not WordUsageView view || value is not WordUsageViewViewModel viewModel)
                {
                    return;
                }

                view.BindingContext = viewModel;
                view.WhenActivated(disposables =>
                {

                });
            });

    // Implementation of IViewFor<WordUsageViewModel>.ViewModel
    public WordUsageViewViewModel? ViewModel
    {
        get => BindingContext as WordUsageViewViewModel ?? null;
        set => BindingContext = value;
    }

    // Explicit implementation of IViewFor.ViewModel
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as WordUsageViewViewModel;
    }

    public WordUsageView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.BindValidation(ViewModel, vm => vm.Text, v => v.TextValidationWrapper.ValidationMessage)
                .DisposeWith(disposables);

            this.BindValidation(ViewModel, vm => vm.Translation, v => v.TranslationValidationWrapper.ValidationMessage)
                .DisposeWith(disposables);
        });
    }
}