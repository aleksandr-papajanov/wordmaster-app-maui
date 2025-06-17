using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMasterApp.Features.MessageContainer
{
    public interface IMessageAction
    {
        string Text { get; set; }
        IObservable<object?> WhenInvoked { get; }
        void Invoke();
    }
}
