using System.Windows.Input;

namespace WordMasterApp.Components;

public class HeaderButton : BindableObject
{
    public static readonly BindableProperty GlyphProperty =
        BindableProperty.Create(nameof(Glyph), typeof(string), typeof(HeaderButton), string.Empty);

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(HeaderButton), null);

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(HeaderButton), null);

    public static readonly BindableProperty FontFamilyProperty =
        BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(HeaderButton), "FontAwesomeSolid");

    public string FontFamily
    {
        get => (string)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public string Glyph
    {
        get => (string)GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object CommandParameter
    {
        get => (object)GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    // New properties for toggle functionality
    public static readonly BindableProperty IsToggledProperty =
        BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(HeaderButton), false, propertyChanged: OnIsToggledChanged);

    public static readonly BindableProperty ToggledCommandProperty =
        BindableProperty.Create(nameof(ToggledCommand), typeof(ICommand), typeof(HeaderButton), null);

    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
    }

    public ICommand ToggledCommand
    {
        get => (ICommand)GetValue(ToggledCommandProperty);
        set => SetValue(ToggledCommandProperty, value);
    }

    

    // Handle toggle state changes
    private static void OnIsToggledChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is HeaderButton headerButton && newValue is bool isToggled)
        {
            // Update the glyph dynamically
            headerButton.OnPropertyChanged(nameof(Glyph));

            // Execute the ToggledCommand if set
            if (headerButton.ToggledCommand?.CanExecute(isToggled) == true)
            {
                headerButton.ToggledCommand.Execute(isToggled);
            }
        }
    }

    public HeaderButton()
    {
    }
}