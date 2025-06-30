using Microsoft.Maui.Controls.Shapes;
using ReactiveUI;
using System.Reactive.Disposables;
using WordMasterApp.Helpers;

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

    private View BuildMessage(IMessageModel msg)
    {
        var frame = new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = 20 },
            Padding = 10,
            BackgroundColor = msg.PrimaryColor
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
            var header = new Border
            {
                BackgroundColor = ThemeExtentions.GetColor("Text"),
                Padding = 5,
                StrokeShape = new RoundRectangle { CornerRadius = 10 },
                HorizontalOptions = LayoutOptions.Start,
                Content = new Label
                {
                    Text = msg.Title,
                    TextColor = msg.PrimaryColor,
                    FontAttributes = FontAttributes.Bold,
                    Style = Application.Current?.Resources["LabelRegular"] as Style
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
            contentStack.Children.Add(new Label
            {
                Text = msg.Message,
                Style = Application.Current?.Resources["LabelRegular"] as Style
            });
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