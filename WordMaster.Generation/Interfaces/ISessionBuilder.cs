using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMaster.Generation.Stages;

namespace WordMaster.Generation.Interfaces
{
    public interface ISessionBuilder
    {
        ISessionBuilder UseStage(string? key, IStage stage);
        ISessionBuilder UseMiddleware(IMiddleware stage);
        ISessionBuilder SetRequest(IRequest request);

        ISessionBuilder UseCustomActionStage(string? key, Func<IContext, ISessionController, Task> action);
        ISessionBuilder UseRetryMiddleware(int retries = 1);
        ISessionBuilder UseWordBasicsCompletionStage(string? key = null);
        ISessionBuilder UseWordDetailsCompletionStage(string? key = null);
        ISessionBuilder UseWordUsageCompletionStage(string? key = null);

        ISession Build();
        void Reset();
    }
}
