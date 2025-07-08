namespace WordMaster.Generation.Interfaces
{
    public interface ISessionController
    {
        Task RepeatStage();
        Task ToNextStage();
        Task FinishAfter(string id);
        Task Cancel();
    }

    public enum SessionControllerAction
    {
        Continue,
        Repeat,
        Cancel
    }
}
