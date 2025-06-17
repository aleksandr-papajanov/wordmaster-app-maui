using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMasterApp.Features.MessageContainer
{
    public interface IMessageService
    {
        IObservable<IMessageModel> Messages { get; }
        void Publish(IMessageModel message);
    }
}
