using OpenAI.Chat;
using ReactiveUI;
using WordMasterApp.Components.LoadingStripe;
using WordMasterApp.Features.MessageContainer;
using WordMasterApp.Messages;

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
        public event Func<IGenerationContextSnapshot, Task> OnComplete;

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

            OnComplete += async (snapshot) =>
            {
                // Ensure the loading stripe is hidden when the session completes
                MessageBus.Current.SendMessage(new LoadingStripeMessage(false));
                await Task.CompletedTask;
            };
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
            MessageBus.Current.SendMessage(new LoadingStripeMessage(true, LoadingStripeType.HttpRequest));

            while (_stages.Count > 0 && Context.SessionResult == null)
            {
                var stage = _stages.Peek();

                await ExecuteInMiddlewarePipeline(stage);

                if (Context.SessionResult != null)
                {
                    if (OnComplete != null)
                    {
                        await OnComplete.Invoke(Context);
                    }
                    
                    return;
                }

                if (stage.RequiresInteraction)
                {
                    _completion = new();
                    OnInteractionRequested?.Invoke(new StageInteractionRequest(stage.GetType().Name, this, Context));
                    await _completion.Task;

                    if (Context.SessionResult != null)
                    {
                        if (OnComplete != null)
                        {
                            await OnComplete.Invoke(Context);
                        }

                        return;
                    }
                }
                else
                {
                    _stages.Dequeue();
                }
            }

            Context.SessionResult = GenerationSessionResult.Success;

            if (OnComplete != null)
            {
                await OnComplete.Invoke(Context);
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

            Context.SessionResult = GenerationSessionResult.Cancelled;

            if (OnComplete != null)
            {
                Context.SessionResult = GenerationSessionResult.Cancelled;
                await OnComplete.Invoke(Context);
            }

            // continue
            _completion?.TrySetResult();
        }
    }
}
