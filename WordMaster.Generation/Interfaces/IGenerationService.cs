using WordMaster.Generation.DTOs;

namespace WordMaster.Generation.Interfaces
{
    public interface IGenerationService
    {
        IGenerationSession CompleteWordBase(WordCompletionRequest request);
        IGenerationSession CompleteWordDetails(WordCompletionRequest request);
    }
}

