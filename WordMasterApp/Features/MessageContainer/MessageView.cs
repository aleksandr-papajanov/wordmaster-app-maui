using ReactiveUI;
using System.Collections.Specialized;
using System.Reactive.Disposables;
using WordMasterApp.Features.MessageContainer;

namespace WordMasterApp.Features.MessageContainer;

public class MessageView : ContentView, IViewFor<IMessageModel>
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(
            nameof(ViewModel),
            typeof(IMessageModel),
            typeof(MessageView));

    public IMessageModel? ViewModel
    {
        get => (IMessageModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (IMessageModel?)value;
    }

    public MessageView()
    {
        this.WhenActivated(disposables =>
        {
            // Авто BindingContext
            this.OneWayBind(this, v => v.ViewModel, v => v.BindingContext)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel)
                .WhereNotNull()
                .Subscribe(message =>
                {
                    Content = BuildMessage(message);
                })
                .DisposeWith(disposables);
        });
    }

    private Color GetColor(string name)
    {
        if (Application.Current == null)
        {
            return Colors.Transparent;
        }

        return Application.Current.RequestedTheme switch
        {
            AppTheme.Dark => Application.Current.Resources[name + "Dark"] is Color dark ? dark : Colors.Red,
            AppTheme.Light => Application.Current.Resources[name + "Light"] is Color light ? light : Colors.Red,
            _ => Colors.Transparent
        };
    }

    private View BuildMessage(IMessageModel msg)
    {
        var primaryColor = msg switch
        {
            NotificationMessage nm => GetColor("Primary"),
            ConfirmationMessage cm => GetColor("Secondary"),
            ErrorMessage em => GetColor("Error"),
            _ => GetColor("Primary")
        };

        var frame = new Frame
        {
            CornerRadius = 20,
            Padding = 10,
            BackgroundColor = primaryColor
        };

        var stack = new VerticalStackLayout
        {
            Spacing = 10,
        };

        var buttons = new HorizontalStackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            Spacing = 10
        };

        foreach (var action in msg.Actions)
        {
            var button = new Button();
            button.SetBinding(Button.TextProperty, new Binding(nameof(MessageAction.Text), source: action));
            button.Clicked += (s, e) => action.Invoke();
            buttons.Children.Add(button);
        }

        if (!string.IsNullOrWhiteSpace(msg.Title))
        {
            var header = new Frame
            {
                BackgroundColor = GetColor("Text"),
                Padding = 5,
                CornerRadius = 10,
                HorizontalOptions = LayoutOptions.Start,
                Content = new Label
                {
                    Text = msg.Title,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = primaryColor,
                }
            };

            stack.Children.Add(header);
        }

        var contentStack = new VerticalStackLayout
        {
            Padding = 10,
        };

        if (!string.IsNullOrWhiteSpace(msg.Message))
        {
            contentStack.Children.Add(new Label { Text = msg.Message });
        }

        if (msg.ExtraContent is View view)
        {
            contentStack.Children.Add(view);
        }

        stack.Children.Add(contentStack);
        stack.Children.Add(buttons);
        frame.Content = stack;

        return frame;
    }

    public Task AnimateAppear()
    {
        this.Opacity = 0;
        this.TranslationY = -50;

        return Task.WhenAll(
            this.FadeTo(1, 500, Easing.CubicIn),
            this.TranslateTo(0, 0, 500, Easing.CubicIn)
        );
    }

    public Task AnimateDisappear()
    {
        return Task.WhenAll(
            this.FadeTo(0, 500, Easing.CubicOut),
            this.TranslateTo(0, 50, 500, Easing.CubicOut)
        );
    }
}