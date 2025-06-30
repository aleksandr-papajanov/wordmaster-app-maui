using WordMaster.Generation.Interfaces;

namespace WordMaster.Generation
{
    internal class RetryMiddleware : IGenerationMiddleware
    {
        private readonly int _maxRetries;

        public RetryMiddleware(int maxRetries = 1)
        {
            _maxRetries = maxRetries;
        }

        public async Task InvokeAsync(IGenerationContext context, IGenerationStage stage, Func<IGenerationContext, Task> next)
        {
            int attempts = 0;

            while (true)
            {
                try
                {
                    await next(context);
                    return;
                }
                catch (Exception) when (++attempts < _maxRetries)
                {
                    await Task.Delay(1000);
                }
                catch (Exception e)
                {
                    context.ErrorMessage = $"{e.Message} (Attempted {attempts}/{_maxRetries})";
                    context.SessionResult = GenerationSessionResult.Error;
                    return;
                }
            }
        }
    }
}
