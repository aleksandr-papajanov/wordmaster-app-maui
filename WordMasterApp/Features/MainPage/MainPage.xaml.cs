using ReactiveUI;
using System.Reactive.Disposables;
using WordMasterApp.Components.LoadingStripe;
using WordMasterApp.Features.MessageContainer;
using WordMasterApp.Messages;

namespace WordMasterApp.Features.MainPage;

public partial class MainPage : ContentPage, IViewFor<MainViewModel>
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(MainViewModel), typeof(MainPage),
            propertyChanged: (bindable, _, value) =>
            {
            });

    // Implementation of IViewFor<>.ViewModel
    public MainViewModel? ViewModel
    {
        get => BindingContext as MainViewModel ?? null;
        set => BindingContext = value;
    }

    // Explicit implementation of IViewFor.ViewModel
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as MainViewModel;
    }


    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;

        this.SizeChanged += (s, e) =>
        {
            MessageContainer.MaximumHeightRequest = 0.8 * Height;
            MessageContainer.MaximumWidthRequest = 0.5 * Width;
        };

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm, view => view.BindingContext)
                .DisposeWith(disposables);

            
            
        });
    }

    //private int i = 0;
    private void Button_Clicked(object sender, EventArgs e)
    {
        ViewModel?.Test();

        //MessageBus.Current.SendMessage(new LoadingStripeMessage(true, (LoadingStripeType)i++));
        //if (i > 3) i = 0;
    }
}
