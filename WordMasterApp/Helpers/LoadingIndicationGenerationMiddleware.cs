using ReactiveUI;
using WordMaster.Generation.Interfaces;
using WordMasterApp.Components.LoadingStripe;
using WordMasterApp.Messages;

namespace WordMasterApp.Helpers
{
    class LoadingIndicationGenerationMiddleware : IMiddleware
    {
        public async Task InvokeAsync(IContext context, ISessionController control, IStage stage, Func<IContext, ISessionController, Task> next)
        {
            MessageBus.Current.SendMessage(new LoadingStripeMessage(true, LoadingStripeType.HttpRequest));

            await next(context, control);

            MessageBus.Current.SendMessage(new LoadingStripeMessage(false));
        }
    }
}