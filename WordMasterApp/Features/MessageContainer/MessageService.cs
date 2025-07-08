using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using WordMasterApp.Features.MessageContainer.Interfaces;

namespace WordMasterApp.Features.MessageContainer
{
    public class MessageService : IMessageService
    {
        private readonly Subject<IMessageViewModel> _messages = new();

        public IObservable<IMessageViewModel> Messages => _messages.AsObservable();

        public void Publish(IMessageViewModel message)
        {
            _messages.OnNext(message);
        }
    }
}
