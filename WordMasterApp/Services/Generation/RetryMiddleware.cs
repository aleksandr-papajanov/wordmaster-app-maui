using WordMasterApp.Features.MessageContainer;

namespace WordMasterApp.Services.Generation
{
    internal class RetryMiddleware : IGenerationMiddleware
    {
        private readonly IMessageService _messageService;
        private readonly int _maxRetries;

        public RetryMiddleware(IMessageService messageService, int maxRetries = 1)
        {
            _maxRetries = maxRetries;
            _messageService = messageService;
        }

        public async Task InvokeAsync(IGenerationContext context, IGenerationStage stage, Func<IGenerationContext, Task> next)
        {
            int attempts = 0;

            while (attempts < _maxRetries)
            {
                try
                {
                    await next(context);
                    return;
                }
                catch (Exception e)
                {
                    if (++attempts < _maxRetries)
                    {
                        await Task.Delay(1000);
                    }
                    else
                    {
                        _messageService.Publish(new ErrorMessage($"{e.Message} (Attempt {attempts}/{_maxRetries})"));
                    }
                }
            }

            context.IsComplete = true;
        }
    }
}
