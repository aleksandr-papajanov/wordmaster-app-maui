namespace WordMasterApp.Services.Generation
{
    public record StageInteractionRequest(
        string StageName,
        IGenerationSessionController SessionControl,
        IGenerationContextSnapshot ContextSnapshot
    );
}
