using WordMaster.Generation.Interfaces;

namespace WordMaster.Generation
{
    public record StageInteractionRequest(
        string StageName,
        ISessionController SessionControl,
        IContext Context
    );
}
