using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WordMasterApp.Features.MessageContainer.Interfaces
{
    public interface IMessageAction
    {
        string Text { get; set; }
        ICommand Invoke { get; }
        IObservable<object?> WhenInvoked { get; }
    }
}
