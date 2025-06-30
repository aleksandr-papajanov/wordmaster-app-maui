namespace WordMaster.Generation.Interfaces
{
    public interface IGenerationMiddleware
    {
        Task InvokeAsync(IGenerationContext context, IGenerationStage stage, Func<IGenerationContext, Task> next);
    }
}
