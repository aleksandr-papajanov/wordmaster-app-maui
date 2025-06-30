namespace WordMaster.Generation.Interfaces
{
    public interface IGenerationSessionController
    {
        Task Repeat();
        Task Continue();
        Task Cancel();
    }
}
