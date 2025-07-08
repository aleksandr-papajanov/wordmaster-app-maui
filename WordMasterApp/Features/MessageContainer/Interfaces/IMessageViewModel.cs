using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WordMasterApp.Features.MessageContainer.Interfaces
{
    public interface IMessageViewModel
    {
        string Title { get; set; }
        string Message { get; set; }
        ObservableCollection<IMessageAction> Actions { get; }
        Task<object?> Completion { get; }
        ICommand CloseCommand { get; }

        event Action? RequestClose;
    }
}
