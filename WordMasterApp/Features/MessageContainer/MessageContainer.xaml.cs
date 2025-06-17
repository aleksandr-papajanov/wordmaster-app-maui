using ReactiveUI;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Disposables;

namespace WordMasterApp.Features.MessageContainer;

public partial class MessageContainer : ContentView, IViewFor<MessageContainerViewModel>
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(
            nameof(ViewModel),
            typeof(MessageContainerViewModel),
            typeof(MessageContainer));

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
            // Авто BindingContext
            this.OneWayBind(this, v => v.ViewModel, v => v.BindingContext)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel)
                .WhereNotNull()
                .Subscribe(vm =>
                {
                    if (vm.Messages is INotifyCollectionChanged coll)
                    {
                        coll.CollectionChanged += (s, e) => RenderMessages();
                    }

                    RenderMessages();
                })
                .DisposeWith(disposables);


            this.WhenAnyValue(v => v.ViewModel)
                .WhereNotNull()
                .Subscribe(vm =>
                {
                    vm.AnimateRemove.RegisterHandler(async interaction =>
                    {
                        var messageView = MessageStack.Children
                            .OfType<MessageView>()
                            .FirstOrDefault(v => v.ViewModel == interaction.Input);

                        if (messageView != null)
                        {
                            await messageView.AnimateDisappear();
                        }

                        interaction.SetOutput(Unit.Default);
                    })
                    .DisposeWith(disposables);

                    vm.AnimateAdd.RegisterHandler(async interaction =>
                    {
                        var messageView = MessageStack.Children
                            .OfType<MessageView>()
                            .FirstOrDefault(v => v.ViewModel == interaction.Input);

                        if (messageView != null)
                        {
                            await messageView.AnimateAppear();
                        }

                        interaction.SetOutput(Unit.Default);
                    })
                    .DisposeWith(disposables);
                })
                .DisposeWith(disposables);
        });
    }

    void RenderMessages()
    {
        if (ViewModel?.Messages == null)
            return;

        MessageStack.Children.Clear();

        foreach (var message in ViewModel.Messages)
        {
            var messageView = new MessageView
            {
                ViewModel = message
            };

            MessageStack.Children.Add(messageView);
        }
    }
}