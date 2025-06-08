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

            MessagingCenter.Subscribe<WordUsageViewViewModel>(this, "ClearCollectionViewSelection", (sender) =>
            {
                // First clear the selected item in the ViewModel
                //if (ViewModel != null)
                //{
                //    ViewModel.SelectedUsage = null;
                //}

                //foreach (var container in UsageCollection.Ite)
                //{
                //    if (container is Grid grid && grid.Style == Resources["UsageItemStyle"])
                //    {
                //        // Force the visual state to Normal
                //        VisualStateManager.GoToState(grid, "Normal");
                //    }
                //}
            });
        });
    }

    //private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    //{
    //    if (sender is not CollectionView view)
    //        return;

    //    // If the selected item is the same as the previously selected item, deselect it
    //    if (e.CurrentSelection.FirstOrDefault() == e.PreviousSelection.FirstOrDefault())
    //    {
    //        view.SelectedItem = null; 
    //    }
    //}
}