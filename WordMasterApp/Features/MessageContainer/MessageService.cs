using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace WordMasterApp.Features.MessageContainer
{
    public class MessageService : IMessageService
    {
        private readonly Subject<IMessageModel> _messages = new();

        public IObservable<IMessageModel> Messages => _messages.AsObservable();

        public void Publish(IMessageModel message)
        {
            _messages.OnNext(message);
        }
    }
}
