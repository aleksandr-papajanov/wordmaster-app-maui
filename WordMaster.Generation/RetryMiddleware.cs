using WordMaster.Generation.Interfaces;

namespace WordMaster.Generation
{
    internal class RetryMiddleware : IMiddleware
    {
        private readonly int _maxRetries;

        public RetryMiddleware(int maxRetries = 1)
        {
            _maxRetries = maxRetries;
        }

        public async Task InvokeAsync(IContext context, ISessionController control, IStage stage, Func<IContext, ISessionController, Task> next)
        {
            int attempts = 0;

            while (true)
            {
                try
                {
                    await next(context, control);
                    return;
                }
                catch (Exception) when (++attempts < _maxRetries)
                {
                    await Task.Delay(1000);
                }
                catch (Exception e)
                {
                    context.ErrorMessage = $"{e.Message} (Attempted {attempts}/{_maxRetries})";
                    context.SessionResult = SessionResult.Error;
                    return;
                }
            }
        }
    }
}
