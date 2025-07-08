namespace WordMaster.Generation.Interfaces
{
    public interface IMiddleware
    {
        Task InvokeAsync(IContext context, ISessionController control, IStage stage, Func<IContext, ISessionController, Task> next);
    }
}
