namespace WordMaster.Generation.Interfaces
{
    public interface ISession
    {
        event Func<IContextSnapshot, Task> OnComplete;
         
        IContext Context { get; }

        Task RunAsync();
    }
}