using OpenAI.Chat;
using WordMaster.Generation;

namespace WordMaster.Generation.Interfaces
{
    public interface IGenerationSession
    {
        event Func<StageInteractionRequest, Task> OnInteractionRequested;
        event Func<IGenerationContextSnapshot, Task> OnComplete;
         
        IGenerationContext Context { get; }
        Task RunAsync();
    }
}
