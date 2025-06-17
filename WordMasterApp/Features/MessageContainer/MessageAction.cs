using System.Reactive.Subjects;

namespace WordMasterApp.Features.MessageContainer
{
    public class MessageAction : IMessageAction
    {
        public string Text { get; set; } = "Action";
        public object? Value { get; set; }

        public IObservable<object?> WhenInvoked => _invoked;
        private readonly Subject<object?> _invoked = new();

        public void Invoke()
        {
            _invoked.OnNext(Value);
            _invoked.OnCompleted();
        }
    }
}
