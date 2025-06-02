using ReactiveUI;
using ReactiveUI.Maui;
using ReactiveUI.Validation.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using WordMasterApp.Converters;
using WordMasterApp.Infrastructure.Interfaces;
using WordMasterApp.ViewModels.Word;

namespace WordMasterApp.Views;

public partial class WordPage : BaseContentPage, IViewFor<WordViewModel>
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(WordViewModel), typeof(WordPage), default(WordViewModel));

    public WordViewModel? ViewModel
    {
        get => (WordViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (WordViewModel)value!;
    }


    public WordPage(WordViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        BindingContext = viewModel;
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();

        var disposableRegistration = new CompositeDisposable();

        this.Bind(ViewModel, vm => vm.SearchText, v => v.SearchEntry.Text)
            .DisposeWith(disposableRegistration);

        this.OneWayBind(ViewModel, vm => vm.WordBlobsViewModel, v => v.BlobCollection.ViewModel)
            .DisposeWith(disposableRegistration);

        this.BindValidation(ViewModel, vm => vm.SelectedWord.Text, v => v.TextErrorLabel.Text)
            .DisposeWith(disposableRegistration);
        
        this.BindValidation(ViewModel, vm => vm.SelectedWord.Translation, v => v.TranslationErrorLabel.Text)
            .DisposeWith(disposableRegistration);

        //this.BindValidation(ViewModel, view => view.FormErrors.Text)
        //    .DisposeWith(disposableRegistration);
    }

    private async void Back_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}