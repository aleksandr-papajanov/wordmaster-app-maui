using OpenAI.Chat;
using System.Text.Json;
using WordMaster.Common;
using WordMaster.Generation.Interfaces;

namespace WordMaster.Generation.Stages
{
    public class WordBaseCompletionStage : IGenerationStage
    {
        private string? _promptAccessor;

        public bool RequiresInteraction { get; }

        public WordBaseCompletionStage(bool requiresInteraction)
        {
            RequiresInteraction = requiresInteraction;
        }

        // Main execution
        public async Task ExecuteAsync(IGenerationContext context)
        {
            _promptAccessor = (string.IsNullOrWhiteSpace(context.Get<string>(WordBaseCompletionKeys.Word)) ? "W0" : "W1") +
                              (string.IsNullOrWhiteSpace(context.Get<string>(WordBaseCompletionKeys.Translation)) ? "T0" : "T1") +
                              (string.IsNullOrWhiteSpace(context.Get<string>(WordBaseCompletionKeys.Definition)) ? "D0" : "D1");

            context.History.Add(new UserChatMessage(GetPrompt(context, false)));

            ChatCompletionOptions options = new()
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: WordBaseCompletionStageConstants.ResonseName,
                    jsonSchema: BinaryData.FromString(SetPlaceholders(context, WordBaseCompletionStageConstants.ResonseSchema)),
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
            if (_promptAccessor == null || !WordBaseCompletionStageConstants.Prompts.TryGetValue(_promptAccessor, out var pair))
            {
                throw new InvalidOperationException("Coudn't get prompt");
            }

            var prompt = isRetry ? pair.RetryPrompt : pair.InitialPrompt;
            prompt += $"Provide also an approximate frequency of use from the predefined list. ";

            return SetPlaceholders(context, prompt);
        }
        private string SetPlaceholders(IGenerationContext context, string text)
            => text.Replace("[Word]", context.Get<string>(WordBaseCompletionKeys.Word))
                   .Replace("[Translation]", context.Get<string>(WordBaseCompletionKeys.Translation))
                   .Replace("[Definition]", context.Get<string>(WordBaseCompletionKeys.Definition))
                   .Replace("[SourceLanguage]", context.Get<string>(WordBaseCompletionKeys.SourceLanguage))
                   .Replace("[TargetLanguage]", context.Get<string>(WordBaseCompletionKeys.TargetLanguare));

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
                if (!root.TryGetProperty("frequency", out var obj) ||
                    !Enum.TryParse<WordUsageFrequenсy>(obj.GetString(), true, out var frequency))
                {
                    throw new Exception("Error parsing response");
                }
                
                context.Set(WordBaseCompletionKeys.Word, root.GetProperty("word").GetString() ?? throw new Exception());
                context.Set(WordBaseCompletionKeys.Translation, root.GetProperty("translation").GetString() ?? throw new Exception());
                context.Set(WordBaseCompletionKeys.Definition, root.GetProperty("definition").GetString() ?? throw new Exception());
                context.Set(WordBaseCompletionKeys.UsageFrequency, frequency);
            }
            catch (Exception)
            {
                throw new Exception("Error parsing response");
            }
        }
    }
}
