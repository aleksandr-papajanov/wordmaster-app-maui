using OpenAI.Chat;

namespace WordMasterApp.Services.Generation
{
    public interface IGenerationSession
    {
        event Func<StageInteractionRequest, Task> OnInteractionRequested;
        event Func<IGenerationContextSnapshot, Task> OnSessionCompleted;
         
        IGenerationContext Context { get; }
        Task RunAsync();
    }
}
