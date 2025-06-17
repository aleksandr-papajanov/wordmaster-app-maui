using OpenAI.Chat;
using System.Text.Json;
using WordMasterApp.DataLayer.Models;

namespace WordMasterApp.Services.Generation.Stages
{
    internal class WordDetailsCompletionStage : IGenerationStage
    {
        public bool RequiresInteraction => false;


        // Main execution
        public async Task ExecuteAsync(IGenerationContext context)
        {
            context.History.Add(new UserChatMessage(WordDetailsCompletionStageConstants.Prompt.InitialPrompt));

            ChatCompletionOptions options = new()
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: WordDetailsCompletionStageConstants.ResonseName,
                    jsonSchema: BinaryData.FromString(SetPlaceholders(context, WordDetailsCompletionStageConstants.ResonseSchema)),
                    jsonSchemaIsStrict: true)
            };

            ChatCompletion completion = await context.Chat.CompleteChatAsync(context.History, options);

            context.History.Add(new AssistantChatMessage(completion));

            ParseResponse(completion.Content[0].Text, context);
        }

        public Task OnControllerActionAsync(GenerationSessionControllerAction action, IGenerationContext context)
        {
            throw new NotImplementedException();
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

                throw new Exception($"Agent failed with: {message}");
            }

            try
            {
                if (!root.TryGetProperty("part_of_speech_category", out var obj) ||
                    !Enum.TryParse<PartOfSpeech>(obj.GetString(), true, out var parsedPartOfSpeech))
                {
                    throw new Exception("Error parsing response");
                }

                if (!root.TryGetProperty("frequency", out obj) ||
                    !Enum.TryParse<WordUsageFrequenсy>(obj.GetString(), true, out var frequency))
                {
                    throw new Exception("Error parsing response");
                }

                if (!root.TryGetProperty("synonyms", out var synonymsElement) || synonymsElement.ValueKind != JsonValueKind.Array)
                {
                    throw new Exception("Error parsing response");
                }

                if (!root.TryGetProperty("antonyms", out var antonymsElement) || antonymsElement.ValueKind != JsonValueKind.Array)
                {
                    throw new Exception("Error parsing response");
                }

                context.Set(WordCompletionKeys.Pronunciation, root.GetProperty("pronunciation").GetString() ?? throw new Exception());
                context.Set(WordCompletionKeys.PartOfSpeechLabel, root.GetProperty("part_of_speech_free").GetString() ?? throw new Exception());
                context.Set(WordCompletionKeys.PartOfSpeechType, parsedPartOfSpeech);
                context.Set(WordCompletionKeys.Synonyms, synonymsElement.EnumerateArray().Select(s => s.GetString() ?? string.Empty).ToList());
                context.Set(WordCompletionKeys.Antonyms, antonymsElement.EnumerateArray().Select(a => a.GetString() ?? string.Empty).ToList());
                context.Set(WordCompletionKeys.UsageFrequency, frequency);
            }
            catch (Exception)
            {
                throw new Exception("Error parsing response");
            }

        }
    }
}
