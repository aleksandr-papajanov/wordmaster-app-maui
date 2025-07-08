using OpenAI.Chat;
using System.Text.Json;
using WordMaster.Common;
using WordMaster.Generation.DTOs;
using WordMaster.Generation.Interfaces;

namespace WordMaster.Generation.Stages
{
    internal class WordUsageCompletionStage : StageBase
    {
        private IContext _context = null!;

        protected override string InitialPrompt =>
            SetPlaceholders("Generate [UsagesCount] natural-sounding sentence(s) in [SourceLanguage] using the word \"[Word]\", along with their translation into [TargetLanguage]. ");

        protected override string RetryPrompt =>
            SetPlaceholders("The previous examples weren't satisfactory. Please try using the word in a different context or rephrase to better match the intended meaning.");

        protected override string ToNextStagePrompt => "The provided suggestion is approved. Let's move on with it";
        protected override string StageName => "WordUsageCompletionStage";
        protected override string ResponseName => "complete_word_usage";
        protected override string ResonseSchema => SetPlaceholders(Schema);


        private string SetPlaceholders(string text)
            => text.Replace("[Word]",           _context.Get<string>(nameof(WordCompletionRequest.Word))           ?? throw new ArgumentNullException(nameof(WordCompletionRequest.Word), "Word cannot be null or empty."))
                   .Replace("[UsagesCount]",    _context.Get<int>(nameof(WordCompletionRequest.UsagesCount)).ToString()) ?? "1"
                   .Replace("[SourceLanguage]", _context.Get<string>(nameof(WordCompletionRequest.SourceLanguage)) ?? throw new ArgumentNullException(nameof(WordCompletionRequest.SourceLanguage), "Source language cannot be null or empty."))
                   .Replace("[TargetLanguage]", _context.Get<string>(nameof(WordCompletionRequest.TargetLanguage)) ?? throw new ArgumentNullException(nameof(WordCompletionRequest.TargetLanguage), "Target language cannot be null or empty."));

        public override async Task ExecuteAsync(IContext context, ISessionController control)
        {
            _context = context;

            await Complete(context);
            EnsureSuccessResponse();
            ParseResponse();

            await control.ToNextStage();
        }

        private void ParseResponse()
        {
            if (string.IsNullOrWhiteSpace(LastResponse))
            {
                throw new Exception("Agent didn't provide a response.");
            }

            using JsonDocument doc = JsonDocument.Parse(LastResponse);
            var root = doc.RootElement;
            
            var examples = root.GetProperty("examples")
                .EnumerateArray()
                .Aggregate(new Dictionary<string, string>(), (dict, item) =>
                {
                    string sentence = item.GetProperty("sentence").GetString() ?? string.Empty;
                    string translation = item.GetProperty("translation").GetString() ?? string.Empty;

                    dict[sentence] = translation;

                    return dict;
                });

            _context.Set(nameof(WordCompletionResponse.Sentences), examples);
        }

        public static readonly string Schema = """
        {
          "type": "object",
          "properties": {
            "success": {
              "type": "boolean",
              "description": "Indicates whether the generation was successful. True means all fields are valid, false means there was an error."
            },
            "error_message": {
              "type": "string",
              "description": "If Success is false, this field must contain a clear explanation of what went wrong."
            },
            "examples": {
              "type": "array",
              "items": {
                "type": "object",
                "properties": {
                  "sentence": {
                    "type": "string",
                    "description": "A naturally sounding sentence in [SourceLanguage] using the word."
                  },
                  "translation": {
                    "type": "string",
                    "description": "The translation of the sentence into [TargetLanguage]."
                  }
                },
                "required": ["sentence", "translation"],
                "additionalProperties": false
              }
            }
          },
          "required": ["success", "error_message", "examples"],
          "additionalProperties": false
        }
        """;
    }
}
