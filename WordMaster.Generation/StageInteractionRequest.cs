using WordMaster.Generation.Interfaces;

namespace WordMaster.Generation
{
    public record StageInteractionRequest(
        string StageName,
        IGenerationSessionController SessionControl,
        IGenerationContextSnapshot ContextSnapshot
    );
}
