using OpenAI.Chat;
using WordMaster.Generation.Interfaces;

namespace WordMaster.Generation
{
    public enum SessionResult
    {
        Success,
        Cancelled,
        Error
    }

    public class Session : ISession, ISessionController
    {
        private readonly ICollection<KeyValuePair<string, IStage>> _stages;
        private readonly ICollection<IMiddleware> _middlewares;
        private int _activePipleineIndex = 0;
        private int _activeControlIntex = 0;  
        private TaskCompletionSource? _completion;

        private IStage CurrentPipleineStage => _stages.ElementAt(_activePipleineIndex).Value;
        private IStage CurrentControlStage => _stages.ElementAt(_activeControlIntex).Value;
        public IContext Context { get; }

        private void UpdateQueue()
        {
            if (++_activePipleineIndex >= _stages.Count)
            {
                Context.SessionResult = SessionResult.Success;
                return;
            }

            if (CurrentPipleineStage is not IInteractionPoint)
            {
                _activeControlIntex = _activePipleineIndex;
            }
        }
        
        public event Func<IContextSnapshot, Task> OnComplete;

        public Session(IContext context, ICollection<IMiddleware> middlewares, ICollection<KeyValuePair<string, IStage>> stages)
        {
            if (stages.Count() == 0)
            {
                throw new ArgumentException("At least one stage is required.", nameof(stages));
            }

            Context = context;
            _middlewares = middlewares;
            _stages = stages;

            Context.History.Clear();
            Context.History.Add(new SystemChatMessage(
                "You are a language assistant. Always reply strictly following the provided JSON schema." +
                "You help generate translations, definitions and examples for words." +
                "Be concise and accurate."));
        }


        public async Task RunAsync()
        {
            while (Context.SessionResult == null)
            {
                _completion = new TaskCompletionSource();

                // Execute a stage
                await ExecuteStage(CurrentPipleineStage);

                // wait for ToNextStage / RepeatStage / Cancel
                await _completion.Task;

                if (Context.SessionResult != null)
                {
                    OnComplete?.Invoke(Context);
                    return;
                }
            }

            //Context.SessionResult = SessionResult.Success;
            //OnComplete?.Invoke(Context);
        }

        private async Task ExecuteStage(IStage stage)
        {
            Func<IContext, ISessionController, Task> handler = stage.ExecuteAsync;

            // Build the middleware pipeline in reverse order
            foreach (var midleware in _middlewares.Reverse())
            {
                var next = handler;
                handler = (context, control) => midleware.InvokeAsync(context, control, stage, next);
            }

            await handler(Context, this);
        }


        // Control actions
        public async Task RepeatStage()
        {
            await CurrentControlStage.OnControllerActionAsync(SessionControllerAction.Repeat, Context);
            _activePipleineIndex = _activeControlIntex;
            _completion?.TrySetResult();
        }

        public async Task ToNextStage()
        {
            await CurrentControlStage.OnControllerActionAsync(SessionControllerAction.Continue, Context);
            UpdateQueue();
            _completion?.TrySetResult();
        }

        public async Task Cancel()
        {
            await CurrentControlStage.OnControllerActionAsync(SessionControllerAction.Cancel, Context);
            _stages.Clear();
            Context.SessionResult = SessionResult.Cancelled;
            _completion?.TrySetResult();
        }

        public Task FinishAfter(string key)
        {
            var list = _stages.ToList();
            var index = list.FindIndex(x => x.Key == key);

            if (index == -1)
                return Task.CompletedTask;

            var keysToRemove = list.Skip(index + 1).Select(x => x.Key).ToList();

            foreach (var k in keysToRemove)
            {
                var item = _stages.First(x => x.Key == k);
                _stages.Remove(item);
            }

            return Task.CompletedTask;
        }
    }
}
