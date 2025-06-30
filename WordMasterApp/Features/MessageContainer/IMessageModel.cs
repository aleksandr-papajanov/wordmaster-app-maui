using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMasterApp.Features.MessageContainer
{
    public interface IMessageModel
    {
        Task<object?> Completion { get; }
        ObservableCollection<IMessageAction> Actions { get; }

        string Title { get; set; }
        string Message { get; set; }
        object? ExtraContent { get; set; }
        Color PrimaryColor { get; set; }
    }
}
