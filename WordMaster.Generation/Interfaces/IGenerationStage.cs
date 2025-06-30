using WordMaster.Generation;

namespace WordMaster.Generation.Interfaces
{
    public interface IGenerationStage
    {
        bool RequiresInteraction { get; }
        Task ExecuteAsync(IGenerationContext context);
        Task OnControllerActionAsync(GenerationSessionControllerAction action, IGenerationContext context);
    }
}
