using ReactiveUI;
using System.Buffers.Text;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using WordMasterApp.Features.MessageContainer.Interfaces;

namespace WordMasterApp.Features.MessageContainer
{
    public class MessageContainerViewModel : ReactiveObject
    {
        private readonly IMessageService _messageService;

        private ObservableCollection<IMessageViewModel> _messages = new();
        public ObservableCollection<IMessageViewModel> Messages
        {
            get => _messages;
            set => this.RaiseAndSetIfChanged(ref _messages, value);
        }

        public Interaction<IMessageViewModel, Unit> AnimateRemove { get; } = new();
        public Interaction<IMessageViewModel, Unit> AnimateAdd { get; } = new();

        public MessageContainerViewModel(IMessageService messageService)
        {
            _messageService = messageService;

            _messageService.Messages
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async message => await Add(message));
        }

        public async Task Add(IMessageViewModel message)
        {
            message.RequestClose += async () => await Remove(message);

            Messages.Add(message);
            await AnimateAdd.Handle(message);
            
            //ObserveMessageAndForget(message);
        }

        private async void ObserveMessageAndForget(IMessageViewModel message)
        {
            try
            {
                await message.Completion;
            }
            catch (Exception)
            {
                
            }

            await Remove(message);
        }

        public async Task Remove(IMessageViewModel message)
        {
            await AnimateRemove.Handle(message);
            Messages.Remove(message);
        }
    }
}
