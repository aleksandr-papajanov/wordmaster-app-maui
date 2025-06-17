namespace WordMasterApp.Services.Generation
{
    public interface IGenerationStage
    {
        bool RequiresInteraction { get; }
        Task ExecuteAsync(IGenerationContext context);
        Task OnControllerActionAsync(GenerationSessionControllerAction action, IGenerationContext context);
    }
}
