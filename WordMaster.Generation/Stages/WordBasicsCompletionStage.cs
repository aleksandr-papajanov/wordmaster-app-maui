using System.Text.Json;
using WordMaster.Common;
using WordMaster.Generation.DTOs;
using WordMaster.Generation.Interfaces;

namespace WordMaster.Generation.Stages
{
    internal partial class WordBasicsCompletionStage : StageBase
    {
        private IContext _context = null!;

        protected override string InitialPrompt
        {
            get
            {
                var pair = GetPrompt();
                var prompt = pair.InitialPrompt;
                prompt += $"Provide also an approximate frequency of use from the predefined list. ";

                return SetPlaceholders(prompt);
            }
        }

        protected override string RetryPrompt
        {
            get
            {
                var pair = GetPrompt();
                var prompt = pair.RetryPrompt;
                prompt += $"Provide also an approximate frequency of use from the predefined list. ";

                return SetPlaceholders(prompt);
            }
        }

        protected override string ToNextStagePrompt => "The provided suggestion is approved. Let's move on with it";
        protected override string StageName => "WordBasicsCompletionStage";
        protected override string ResponseName => "complete_word_basics";
        protected override string ResonseSchema => SetPlaceholders(Schema);


        private PromptPair GetPrompt()
        {
            var accessor = (string.IsNullOrWhiteSpace(_context.Get<string>(nameof(WordCompletionRequest.Word))) ? "W0" : "W1") +
                           (string.IsNullOrWhiteSpace(_context.Get<string>(nameof(WordCompletionRequest.Translation))) ? "T0" : "T1") +
                           (string.IsNullOrWhiteSpace(_context.Get<string>(nameof(WordCompletionRequest.Definition))) ? "D0" : "D1");

            if (!Prompts.TryGetValue(accessor, out var pair))
            {
                throw new InvalidOperationException("Coudn't get prompt");
            }

            return pair;
        }

        private string SetPlaceholders(string text)
            => text.Replace("[Word]",           _context.Get<string>(nameof(WordCompletionRequest.Word))           ?? throw new ArgumentNullException(nameof(WordCompletionRequest.Word), "Word cannot be null or empty."))
                   .Replace("[Translation]",    _context.Get<string>(nameof(WordCompletionRequest.Translation))    ?? throw new ArgumentNullException(nameof(WordCompletionRequest.Translation), "Translation cannot be null or empty."))
                   .Replace("[Definition]",     _context.Get<string>(nameof(WordCompletionRequest.Definition))     ?? throw new ArgumentNullException(nameof(WordCompletionRequest.Definition), "Definition cannot be null or empty."))
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

            if (!root.TryGetProperty("frequency", out var obj) ||
                !Enum.TryParse<WordUsageFrequenсy>(obj.GetString(), true, out var frequency))
            {
                throw new Exception("Frequency property is missing or invalid.");
            }
                
            _context.Set(nameof(WordCompletionResponse.Word),        root.GetProperty("word").GetString()        ?? throw new Exception("Word property is missing or empty."));
            _context.Set(nameof(WordCompletionResponse.Translation), root.GetProperty("translation").GetString() ?? throw new Exception("Translation property is missing or empty."));
            _context.Set(nameof(WordCompletionResponse.Definition),  root.GetProperty("definition").GetString()  ?? throw new Exception("Definition property is missing or empty."));
            _context.Set(nameof(WordCompletionResponse.UsageFrequency), frequency);
        }

        private static readonly string Schema = """
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
            "word": {
              "type": "string",
              "description": "The main word in [SourceLanguage]."
            },
            "translation": {
              "type": "string",
              "description": "The translation of the word into [TargetLanguage]."
            },
            "definition": {
              "type": "string",
              "description": "A clear and concise definition of the word in [SourceLanguage]."
            },
            "frequency": {
              "type": "string",
              "enum": [
                "rare",
                "uncommon",
                "common",
                "frequent",
                "veryFrequent"
              ],
              "description": "An approximate indication of how frequently the word is used."
            }
          },
          "required": ["success", "error_message", "word", "translation", "definition", "frequency"],
          "additionalProperties": false
        }
        """;

        public readonly static Dictionary<string, PromptPair> Prompts = new()
        {
            { "W1T0D0", new(
                $"I have a word in [SourceLanguage]: [Word]. Please generate the most common translation into [TargetLanguage] and a clear definition. ",
                "The previous translation and definition were not good enough. Please try a better translation into [TargetLanguage] and clearer definition. "
            )},
            { "W0T1D0", new(
                $"I have a translation: [Translation] into [TargetLanguage]. Please find the best matching word in [SourceLanguage] and a clear definition. ",
                $"The previous word and definition did not fit well. Please find a better matching word in [SourceLanguage] and clearer definition. "
            )},
            { "W0T0D1", new(
                $"I have a text description: [Definition]. Please understand its meaning and suggest the best matching word in [SourceLanguage], translation into [TargetLanguage] and definition. ",
                $"The previous word and translation were not good enough. Please understand the meaning again and try to suggest a better word in [SourceLanguage], its translation into [TargetLanguage] and a clearer definition."
            )},
            { "W1T1D0", new (
                $"I have a word [SourceLanguage]: [Word] and translation into [TargetLanguage]: [Translation]. Please generate a clear definition. ",
                $"The previous definition was not clear enough. Please generate a clearer and more precise definition. "
            )},
            { "W1T0D1", new(
                $"I have a word [SourceLanguage]: [Word] and a description: [Definition]. Please generate the best translation into [TargetLanguage] and refine the definition if needed. ",
                $"The previous translation and refined definition were not satisfying. Please try a better translation into [TargetLanguage] and improve the definition if needed ."
            )},

            { "W0T1D1", new (
                $"I have a translation into [TargetLanguage]: [Translation] and description: [Definition]. Please find the best matching word in [SourceLanguage] and adjust translation/definition if needed. ",
                $"The previous word or adjustments were not correct. Please find a better matching word in [SourceLanguage] and adjust the translation/definition if needed. "
            )},
            { "W1T1D1", new (
                $"I have a word [SourceLanguage]: [Word], translation into [TargetLanguage]: [Translation], and definition: [Definition]. ",
                $"The previous version was not satisfying. Please refine all parts to be clearer and more accurate. "
            )},
        };
    }
}
