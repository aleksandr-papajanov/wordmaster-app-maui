using OpenAI.Chat;

namespace WordMasterApp.Services.Generation
{
    public enum GenerationSessionControllerAction
    {
        Continue,
        Repeat,
        Cancel
    }

    public class GenerationSession : IGenerationSession, IGenerationSessionController
    {
        private readonly Queue<IGenerationStage> _stages = new();
        private readonly IList<IGenerationMiddleware> _middlewares;
        private TaskCompletionSource? _completion;
        public IGenerationContext Context { get; }
        
        public event Func<StageInteractionRequest, Task> OnInteractionRequested;
        public event Func<IGenerationContextSnapshot, Task> OnSessionCompleted;

        public GenerationSession(IGenerationContext context, IEnumerable<IGenerationMiddleware> middlewares, IEnumerable<IGenerationStage> stages)
        {
            if (stages.Count() == 0)
            {
                throw new ArgumentException("At least one stage is required.", nameof(stages));
            }

            Context = context;
            _middlewares = middlewares.ToList();
            _stages = new(stages);

            Context.History.Clear();
            Context.History.Add(new SystemChatMessage(
                "You are a language assistant. Always reply strictly following the provided JSON schema." +
                "You help generate translations, definitions and examples for words." +
                "Be concise and accurate."));
        }


        private async Task ExecuteInMiddlewarePipeline(IGenerationStage stage)
        {
            Func<IGenerationContext, Task> handler = (context) => stage.ExecuteAsync(context);

            foreach (var midleware in _middlewares.Reverse())
            {
                var next = handler;
                handler = (context) => midleware.InvokeAsync(context, stage, next);
            }

            await handler(Context);
        }

        public async Task RunAsync()
        {
            while (_stages.Count > 0 && !Context.IsComplete)
            {
                var stage = _stages.Peek();

                await ExecuteInMiddlewarePipeline(stage);

                if (Context.IsComplete)
                {
                    return;
                }

                if (stage.RequiresInteraction)
                {
                    _completion = new();
                    OnInteractionRequested?.Invoke(new StageInteractionRequest(stage.GetType().Name, this, Context));
                    await _completion.Task;

                    if (Context.IsComplete)
                    {
                        return;
                    }
                }
                else
                {
                    _stages.Dequeue();
                }
            }

            Context.IsComplete = true;

            if (OnSessionCompleted != null)
            {
                await OnSessionCompleted.Invoke(Context);
            }
        }

        public async Task Repeat()
        {
            await _stages
                .Peek()
                .OnControllerActionAsync(GenerationSessionControllerAction.Repeat, Context);

            // continue
            _completion?.TrySetResult();
        }

        public async Task Continue()
        {
            await _stages
                .Dequeue()
                .OnControllerActionAsync(GenerationSessionControllerAction.Continue, Context);

            // continue
            _completion?.TrySetResult();
        }

        public async Task Cancel()
        {
            await _stages
                .Peek()
                .OnControllerActionAsync(GenerationSessionControllerAction.Cancel, Context);

            _stages.Clear();
            Context.IsComplete = true;

            // continue
            _completion?.TrySetResult();
        }
    }
}
