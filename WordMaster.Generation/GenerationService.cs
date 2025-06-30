using OpenAI.Chat;
using WordMaster.Generation.DTOs;
using WordMaster.Generation.Interfaces;
using WordMaster.Generation.Stages;
using WordMaster.Generation;

namespace WordMaster.Generation
{
    public class GenerationService : IGenerationService
    {
        private readonly ChatClient _client;

        public GenerationService(ChatClient client)
        {
            _client = client;
        }

        public IGenerationSession CompleteWordBase(WordCompletionRequest request)
        {
            var context = new GenerationContext(_client);

            context.Set(WordBaseCompletionKeys.Word, request.Word);
            context.Set(WordBaseCompletionKeys.Translation, request.Translation);
            context.Set(WordBaseCompletionKeys.Definition, request.Definition);
            context.Set(WordBaseCompletionKeys.SourceLanguage, request.SourceLanguage);
            context.Set(WordBaseCompletionKeys.TargetLanguare, request.TargetLanguage);

            var session = new GenerationSession(
                context,
                [new RetryMiddleware()],
                [new WordBaseCompletionStage(false)]);

            return session;
        }

        public IGenerationSession CompleteWordDetails(WordCompletionRequest request)
        {
            var context = new GenerationContext(_client);

            context.Set(WordBaseCompletionKeys.Word, request.Word);
            context.Set(WordBaseCompletionKeys.Translation, request.Translation);
            context.Set(WordBaseCompletionKeys.Definition, request.Definition);
            context.Set(WordBaseCompletionKeys.SourceLanguage, request.SourceLanguage);
            context.Set(WordBaseCompletionKeys.TargetLanguare, request.TargetLanguage);

            var session = new GenerationSession(
                context,
                [new RetryMiddleware()],
                [new WordBaseCompletionStage(true), new WordDetailsCompletionStage()]);

            return session;
        }
    }
}
