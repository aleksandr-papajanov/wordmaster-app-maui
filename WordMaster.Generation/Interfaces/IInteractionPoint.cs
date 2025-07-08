using WordMaster.Generation;

namespace WordMaster.Generation.Interfaces
{
    public interface IInteractionPoint
    {
        Task ExecuteAsync(IContext context, ISessionController control);
    }
}
