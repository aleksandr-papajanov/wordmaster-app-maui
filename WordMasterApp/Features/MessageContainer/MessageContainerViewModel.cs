using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;

namespace WordMasterApp.Features.MessageContainer
{
    public class MessageContainerViewModel : ReactiveObject
    {
        private readonly IMessageService _messageService;

        private ObservableCollection<IMessageModel> _messages = new();
        public ObservableCollection<IMessageModel> Messages
        {
            get => _messages;
            set => this.RaiseAndSetIfChanged(ref _messages, value);
        }

        public Interaction<IMessageModel, Unit> AnimateRemove { get; } = new();
        public Interaction<IMessageModel, Unit> AnimateAdd { get; } = new();

        public MessageContainerViewModel(IMessageService messageService)
        {
            _messageService = messageService;

            _messageService.Messages
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async message => await Add(message));
        }

        public async Task Add(IMessageModel message)
        {
            Messages.Add(message);
            await AnimateAdd.Handle(message);
            
            ObserveMessageAndForget(message);
        }

        private async void ObserveMessageAndForget(IMessageModel message)
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

        public async Task Remove(IMessageModel message)
        {
            await AnimateRemove.Handle(message);
            Messages.Remove(message);
        }
    }
}
