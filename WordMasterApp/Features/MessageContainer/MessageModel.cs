using System.Collections.ObjectModel;

namespace WordMasterApp.Features.MessageContainer
{
    public class MessageModel : IMessageModel
    {
        private readonly TaskCompletionSource<object?> _completionSource = new();
        public Task<object?> Completion => _completionSource.Task;

        public ObservableCollection<IMessageAction> Actions { get; } = new();

        public string Title { get; set; }
        public string Message { get; set; }
        public object? ExtraContent { get; set; }
        public Color PrimaryColor { get; set; } = Colors.Transparent;

        public MessageModel()
        {
            Actions.CollectionChanged += (_, _) =>
            {
                foreach (var action in Actions)
                {
                    action.WhenInvoked.Subscribe(result =>
                    {
                        if (!_completionSource.Task.IsCompleted)
                        {
                            _completionSource.TrySetResult(result);
                        }
                    });
                }
            };
        }

        public void Close()
        {
            if (!_completionSource.Task.IsCompleted)
            {
                _completionSource.TrySetResult(null);
            }
        }
    }
}
