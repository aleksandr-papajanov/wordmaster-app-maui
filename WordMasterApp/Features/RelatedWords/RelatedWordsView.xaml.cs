using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace WordMasterApp.Features.RelatedWords;

public partial class RelatedWordsView : ContentView, IViewFor<RelatedWordsViewViewModel>
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(RelatedWordsViewViewModel), typeof(RelatedWordsView), default(RelatedWordsViewViewModel),
            propertyChanged: (bindable, _, value) =>
            {
                if (bindable is not RelatedWordsView view || value is not RelatedWordsViewViewModel viewModel)
                    return;

                view.ViewModel = viewModel;
                view.BindingContext = viewModel;
            });

    public RelatedWordsViewViewModel? ViewModel
    {
        get => (RelatedWordsViewViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as RelatedWordsViewViewModel;
    }

    public RelatedWordsView()
    {
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