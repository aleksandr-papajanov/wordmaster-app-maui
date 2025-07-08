using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMasterApp.Features.MessageContainer.Interfaces
{
    public interface IMessageService
    {
        IObservable<IMessageViewModel> Messages { get; }
        void Publish(IMessageViewModel message);
    }
}
