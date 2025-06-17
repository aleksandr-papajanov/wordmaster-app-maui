using OpenAI.Chat;
using System.Text.Json;

namespace WordMasterApp.Services.Generation.Stages
{
    internal class WordCompletionStage : IGenerationStage
    {
        private string? _promptAccessor;

        public bool RequiresInteraction => true;

        // Main execution
        public async Task ExecuteAsync(IGenerationContext context)
        {
            _promptAccessor = (string.IsNullOrWhiteSpace(context.Get<string>(WordCompletionKeys.Word)) ? "W0" : "W1") +
                              (string.IsNullOrWhiteSpace(context.Get<string>(WordCompletionKeys.Translation)) ? "T0" : "T1") +
                              (string.IsNullOrWhiteSpace(context.Get<string>(WordCompletionKeys.Definition)) ? "D0" : "D1");

            context.History.Add(new UserChatMessage(GetPrompt(context, false)));

            ChatCompletionOptions options = new()
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: WordCompletionStageConstants.ResonseName,
                    jsonSchema: BinaryData.FromString(SetPlaceholders(context, WordCompletionStageConstants.ResonseSchema)),
                    jsonSchemaIsStrict: true)
            };

            ChatCompletion completion = await context.Chat.CompleteChatAsync(context.History, options);

            context.History.Add(new AssistantChatMessage(completion));

            ParseResponse(completion.Content[0].Text, context);
        }

        // Interaction execution
        public Task OnControllerActionAsync(GenerationSessionControllerAction action, IGenerationContext context)
        {
            if (action == GenerationSessionControllerAction.Repeat)
            {
                context.History.Add(new UserChatMessage(GetPrompt(context, true)));
            }
            else if (action == GenerationSessionControllerAction.Continue)
            {
                context.History.Add(new UserChatMessage("The provided suggestion is approved. Let's move on with it"));
            }

            return Task.CompletedTask;
        }


        private string GetPrompt(IGenerationContext context, bool isRetry)
        {
            if (_promptAccessor == null || !WordCompletionStageConstants.Prompts.TryGetValue(_promptAccessor, out var pair))
            {
                throw new InvalidOperationException("Coudn't get prompt");
            }

            return SetPlaceholders(context, isRetry ? pair.RetryPrompt : pair.InitialPrompt);
        }
        private string SetPlaceholders(IGenerationContext context, string text)
            => text.Replace("[Word]", context.Get<string>(WordCompletionKeys.Word))
                   .Replace("[Translation]", context.Get<string>(WordCompletionKeys.Translation))
                   .Replace("[Definition]", context.Get<string>(WordCompletionKeys.Definition))
                   .Replace("[SourceLanguage]", context.Get<string>(WordCompletionKeys.SourceLanguage))
                   .Replace("[TargetLanguage]", context.Get<string>(WordCompletionKeys.TargetLanguare));

        private void ParseResponse(string response, IGenerationContext context)
        {
            using JsonDocument doc = JsonDocument.Parse(response);
            var root = doc.RootElement;

            if (!root.TryGetProperty("success", out var success) || !success.GetBoolean())
            {
                var message = root.TryGetProperty("error_message", out var error)
                    ? error.GetString()
                    : "Unknown error.";

                throw new Exception($"Error parsing response: {message}");
            }

            try
            {
                context.Set(WordCompletionKeys.Word, root.GetProperty("word").GetString() ?? throw new Exception());
                context.Set(WordCompletionKeys.Translation, root.GetProperty("translation").GetString() ?? throw new Exception());
                context.Set(WordCompletionKeys.Definition, root.GetProperty("definition").GetString() ?? throw new Exception());
            }
            catch (Exception)
            {
                throw new Exception("Error parsing response");
            }
        }
    }
}
