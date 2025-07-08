using ReactiveUI;
using System.Reactive.Linq;
using WordMaster.Generation.Interfaces;

namespace WordMaster.Generation.Stages
{
    public class InteractionStage<TOutput> : IStage, IInteractionPoint
    {
        private readonly Interaction<IContext, TOutput> _interaction;
        private readonly Func<IContext, ISessionController, TOutput, Task>? _onResult;

        public string Id { get; init; }

        public InteractionStage(
            Interaction<IContext, TOutput> interaction,
            Func<IContext, ISessionController, TOutput, Task>? onResult = null)
        {
            _interaction = interaction;
            _onResult = onResult;
        }

        

        public async Task ExecuteAsync(IContext context, ISessionController control)
        {
            var result = await _interaction.Handle(context);

            if (_onResult is not null)
            {
                await _onResult(context, control, result);
            }
        }

        public Task OnControllerActionAsync(SessionControllerAction action, IContext context)
        {
            throw new NotImplementedException();
        }
    }
}
