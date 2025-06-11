using System.Collections.ObjectModel;
using System.Windows.Input;

namespace WordMasterApp.Components.Header;

public partial class Header : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(Header), string.Empty);

    public static readonly BindableProperty ButtonsProperty =
        BindableProperty.Create(nameof(Buttons), typeof(ObservableCollection<HeaderButton>), typeof(Header),
            defaultValueCreator: bindable => new ObservableCollection<HeaderButton>());

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public ObservableCollection<HeaderButton> Buttons
    {
        get => (ObservableCollection<HeaderButton>)GetValue(ButtonsProperty);
        set => SetValue(ButtonsProperty, value);
    }

    public Header()
    {
        InitializeComponent();
    }
}