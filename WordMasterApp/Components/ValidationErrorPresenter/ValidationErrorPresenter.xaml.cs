using ReactiveUI;
using ReactiveUI.Validation;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.Helpers;
using System.Reactive.Linq;

namespace WordMasterApp.Components;

public partial class ValidationErrorPresenter : ContentView
{

    public static readonly BindableProperty ValidationProperty =
        BindableProperty.Create(nameof(Validation), typeof(ValidationHelper), typeof(ValidationErrorPresenter), null, propertyChanged: OnValidationChanged);

    public ValidationHelper Validation
    {
        get => (ValidationHelper)GetValue(ValidationProperty);
        set => SetValue(ValidationProperty, value);
    }

    private static void OnValidationChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not ValidationErrorPresenter presenter || newValue is not IValidationComponent validation)
            return;

        validation.ValidationStatusChange
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(status =>
            {
                presenter.Content = new Label
                {
                    Text = status.Text.ToSingleLine(), 
                    TextColor = Colors.Red,
                    IsVisible = !string.IsNullOrWhiteSpace(status.Text.ToSingleLine()),
                    FontSize = 12
                };
            });
    }

    public ValidationErrorPresenter()
    {
        InitializeComponent();
    }
}