using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMaster.Generation.Interfaces;

namespace WordMaster.Generation.Stages
{
    internal class CustomActionStage : IStage
    {
        private readonly Func<IContext, ISessionController, Task> _action;

        public CustomActionStage(Func<IContext, ISessionController, Task> action)
        {
            _action = action;
        }

        public async Task ExecuteAsync(IContext context, ISessionController control)
        {
            await _action(context, control);
            await control.ToNextStage();
        }

        public Task OnControllerActionAsync(SessionControllerAction action, IContext context)
        {
            return Task.CompletedTask;
        }
    }
}
