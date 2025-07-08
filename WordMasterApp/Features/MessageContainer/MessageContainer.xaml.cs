using DynamicData;
using ReactiveUI;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Disposables;

namespace WordMasterApp.Features.MessageContainer;

public partial class MessageContainer : ContentView, IViewFor<MessageContainerViewModel>
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(MessageContainerViewModel), typeof(MessageContainer),
            propertyChanged: (bindable, _, value) =>
            {
                if (bindable is not MessageContainer view || value is not MessageContainerViewModel viewModel)
                    return;

                view.ViewModel = viewModel;
                view.BindingContext = viewModel;
            });

    public MessageContainerViewModel? ViewModel
    {
        get => (MessageContainerViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (MessageContainerViewModel?)value;
    }

    public MessageContainer()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(v => v.ViewModel)
                .WhereNotNull()
                .Subscribe(vm =>
                {
                    vm.AnimateRemove.RegisterHandler(async interaction =>
                    {
                        var messageView = MessageStack.Children
                            .OfType<View>()
                            .FirstOrDefault(v => v.BindingContext == interaction.Input);

                        if (messageView != null)
                        {
                            await AnimateDisappear(messageView);
                        }

                        interaction.SetOutput(Unit.Default);
                    })
                    .DisposeWith(disposables);

                    vm.AnimateAdd.RegisterHandler(async interaction =>
                    {
                        var messageView = MessageStack.Children
                            .OfType<View>()
                            .FirstOrDefault(v => v.BindingContext == interaction.Input);

                        if (messageView != null)
                        {
                            await AnimateAppear(messageView);
                        }

                        interaction.SetOutput(Unit.Default);
                    })
                    .DisposeWith(disposables);
                })
                .DisposeWith(disposables);
        });
    }

    public Task AnimateAppear(View message)
    {
        message.Opacity = 0;
        message.TranslationY = -50;

        return Task.WhenAll(
            message.FadeTo(1, 500, Easing.CubicIn),
            message.TranslateTo(0, 0, 500, Easing.CubicIn)
        );
    }

    public Task AnimateDisappear(View message)
    {
        return Task.WhenAll(
            message.FadeTo(0, 500, Easing.CubicOut),
            message.TranslateTo(0, 50, 500, Easing.CubicOut)
        );
    }
}