using ReactiveUI;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WordMasterApp.Features.MessageContainer.Interfaces;

namespace WordMasterApp.Features.MessageContainer.Messages
{
    public abstract class MessageViewModel : IMessageViewModel
    {
        private TaskCompletionSource<object?> _completionSource = null!;

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public ObservableCollection<IMessageAction> Actions { get; } = new();
        public bool CloseOnAction { get; set; } = true;

        public Task<object?> Completion
        {
            get
            {
                if (_completionSource == null || _completionSource.Task.IsCompleted)
                {
                    _completionSource = new();
                }

                return _completionSource.Task;
            }
        }
        public ICommand CloseCommand { get; }

        // Observed by container to run animations and remove message
        public event Action? RequestClose;

        public MessageViewModel()
        {
            CloseCommand = ReactiveCommand.Create(() => RequestClose?.Invoke());

            Actions.CollectionChanged += (_, _) =>
            {
                foreach (var action in Actions)
                {
                    action.WhenInvoked.Subscribe(result =>
                    {
                        if (_completionSource != null && !_completionSource.Task.IsCompleted)
                        {
                            _completionSource.TrySetResult(result);
                        }

                        if (CloseOnAction)
                        {
                            RequestClose?.Invoke();
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

            RequestClose?.Invoke();
        }
    }
}
