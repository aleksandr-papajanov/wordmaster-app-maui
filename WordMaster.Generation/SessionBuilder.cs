using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMaster.Generation.DTOs;
using WordMaster.Generation.Interfaces;
using WordMaster.Generation.Stages;

namespace WordMaster.Generation
{
    public class SessionBuilder : ISessionBuilder
    {
        private readonly ChatClient _client;
        private readonly ICollection<KeyValuePair<string, IStage>> _stages = [];
        private readonly ICollection<IMiddleware> _middlewares = [];
        private readonly ICollection<IRequest> _requests = [];

        public SessionBuilder(ChatClient client)
        {
            _client = client;
        }

        public ISessionBuilder UseStage(string? key, IStage stage)
        {
            _stages.Add(new (key ?? Guid.NewGuid().ToString(), stage));
            return this;
        }

        public ISessionBuilder UseMiddleware(IMiddleware stage)
        {
            _middlewares.Add(stage);
            return this;
        }

        public ISessionBuilder SetRequest(IRequest request)
        {
            _requests.Add(request);
            return this;
        }

        public ISessionBuilder UseCustomActionStage(string? key, Func < IContext, ISessionController, Task> action)
            => UseStage(key, new CustomActionStage(action));

        public ISessionBuilder UseRetryMiddleware(int retries = 1)
            => UseMiddleware(new RetryMiddleware(retries));

        public ISessionBuilder UseWordBasicsCompletionStage(string? key = null)
            => UseStage(key, new WordBasicsCompletionStage());

        public ISessionBuilder UseWordDetailsCompletionStage(string? key = null)
            => UseStage(key, new WordDetailsCompletionStage());

        public ISessionBuilder UseWordUsageCompletionStage(string? key = null)
            => UseStage(key, new WordUsageCompletionStage());

        public ISession Build()
        {
            var context = new Context(_client);

            foreach (var request in _requests)
            {
                request.SetToContext(context);
            }

            return new Session(context, _middlewares, _stages);
        }

        public void Reset()
        {
            _stages.Clear();
            _middlewares.Clear();
        }
    }
}
