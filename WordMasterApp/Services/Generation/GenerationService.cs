using OpenAI.Chat;
using WordMasterApp.Features.MessageContainer;
using WordMasterApp.Services.Generation.Stages;

namespace WordMasterApp.Services.Generation
{
    

    public class GenerationService : IGenerationService
    {
        private readonly ChatClient _client;
        private readonly IMessageService _messageService;

        public GenerationService(ChatClient client, IMessageService messageService)
        {
            _client = client;
            _messageService = messageService;
        }
        
        public IGenerationSession CompleteWordDetails(WordCompletionState request)
        {
            var context = new GenerationContext(_client);

            context.Set(WordCompletionKeys.Word, request.Word);
            context.Set(WordCompletionKeys.Translation, request.Translation);
            context.Set(WordCompletionKeys.Definition, request.Definition);
            context.Set(WordCompletionKeys.SourceLanguage, request.SourceLanguage);
            context.Set(WordCompletionKeys.TargetLanguare, request.TargetLanguage);

            var session = new GenerationSession(
                context,
                [ new RetryMiddleware(_messageService) ],
                [ new WordCompletionStage(), new WordDetailsCompletionStage() ]);

            return session;
        }
    }
}
