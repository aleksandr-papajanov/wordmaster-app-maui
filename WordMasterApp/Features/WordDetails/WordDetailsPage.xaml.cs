using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using System.Reactive.Disposables;

namespace WordMasterApp.Features;

public partial class WordDetailsPage : ContentPage, IViewFor<WordDetailsViewModel>
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(WordDetailsViewModel), typeof(WordDetailsPage),
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


    public WordDetailsPage(WordDetailsViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        BindingContext = viewModel;

        this.WhenActivated(disposables =>
        {
            this.Bind(ViewModel, vm => vm.SearchText, v => v.SearchEntry.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.WordBlobsVM, v => v.BlobCollection.ViewModel)
                .DisposeWith(disposables);

            this.BindValidation(ViewModel, vm => vm.SelectedWord.Text, v => v.TextErrorLabel.Text)
                .DisposeWith(disposables);

            this.BindValidation(ViewModel, vm => vm.SelectedWord.Translation, v => v.TranslationErrorLabel.Text)
                .DisposeWith(disposables);
        });
    }

    private async void Back_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}