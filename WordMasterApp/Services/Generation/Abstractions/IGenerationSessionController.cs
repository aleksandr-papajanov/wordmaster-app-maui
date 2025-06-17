namespace WordMasterApp.Services.Generation
{
    public interface IGenerationSessionController
    {
        Task Repeat();
        Task Continue();
        Task Cancel();
    }
}
