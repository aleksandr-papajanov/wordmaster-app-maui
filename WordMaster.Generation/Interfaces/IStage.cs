using WordMaster.Generation;

namespace WordMaster.Generation.Interfaces
{
    public interface IStage
    {
        Task ExecuteAsync(IContext context, ISessionController control);
        Task OnControllerActionAsync(SessionControllerAction action, IContext context);
    }
}
