using ReactiveUI;
using System.Reactive.Subjects;
using System.Windows.Input;
using WordMasterApp.Features.MessageContainer.Interfaces;

namespace WordMasterApp.Features.MessageContainer
{
    public class MessageAction : IMessageAction
    {
        private readonly Subject<object?> _invoked = new();

        public string Text { get; set; } = string.Empty;
        public ICommand Invoke { get; }
        public object? Value { get; set; }
        public IObservable<object?> WhenInvoked => _invoked;

        public MessageAction()
        {
            Invoke = ReactiveCommand.Create(() =>
            {
                _invoked.OnNext(Value);
                //_invoked.OnCompleted();
            });
        }
    }
}
