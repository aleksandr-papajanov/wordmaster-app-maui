using WordMasterApp.Services.Generation.Stages;

namespace WordMasterApp.Services.Generation
{
    public interface IGenerationService
    {
        IGenerationSession CompleteWordDetails(WordCompletionState request);
    }
}
