public class ValidationEntry : ContentView
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(ValidationEntry), string.Empty);

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(ValidationEntry), string.Empty);

    public static readonly BindableProperty ValidationMessageProperty =
        BindableProperty.Create(nameof(ValidationMessage), typeof(string), typeof(ValidationEntry), string.Empty);

    public static readonly BindableProperty ValidationIsVisibleProperty =
        BindableProperty.Create(nameof(ValidationIsVisible), typeof(bool), typeof(ValidationEntry), false);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public string ValidationMessage
    {
        get => (string)GetValue(ValidationMessageProperty);
        set => SetValue(ValidationMessageProperty, value);
    }

    public bool ValidationIsVisible
    {
        get => (bool)GetValue(ValidationIsVisibleProperty);
        set => SetValue(ValidationIsVisibleProperty, value);
    }

    public ValidationEntry()
    {
        ControlTemplate = (ControlTemplate)Application.Current.Resources["ValidationEntryTemplate"];
    }
}