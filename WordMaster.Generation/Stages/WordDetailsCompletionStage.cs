using OpenAI.Chat;
using System.Text.Json;
using WordMaster.Common;
using WordMaster.Generation.DTOs;
using WordMaster.Generation.Interfaces;

namespace WordMaster.Generation.Stages
{
    internal class WordDetailsCompletionStage : StageBase
    {
        private IContext _context = null!;

        protected override string InitialPrompt =>
            $"Based on this, generate the following additional details:\r\n" +
            $"- Pronunciation guide, if available.\r\n" +
            $"- A clear free-text part of speech label (e.g., 'phrasal verb', 'modal verb', etc.).\r\n" +
            $"- Select the best matching part of speech category from the predefined list.\r\n" +
            $"- Generate a list of common synonyms for this word in [SourceLanguage].\r\n" +
            $"- Generate a list of common antonyms for this word in [SourceLanguage].\r\n";

        protected override string RetryPrompt =>
            "The previous additional details were not satisfactory. " +
            "Please refine the pronunciation, part of speech labels, synonyms, and antonyms to be more precise and natural.";

        protected override string ToNextStagePrompt => "The provided suggestion is approved. Let's move on with it";
        protected override string StageName => "WordDetailsCompletionStage";
        protected override string ResponseName => "complete_word_details";
        protected override string ResonseSchema => SetPlaceholders(Schema);


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

            if (!root.TryGetProperty("part_of_speech_category", out var obj) ||
                !Enum.TryParse<PartOfSpeech>(obj.GetString(), true, out var parsedPartOfSpeech))
            {
                throw new Exception("Part of speech category not found or invalid in response.");
            }

            if (!root.TryGetProperty("synonyms", out var synonymsElement) || synonymsElement.ValueKind != JsonValueKind.Array)
            {
                throw new Exception("Synonyms not found or invalid in response.");
            }

            if (!root.TryGetProperty("antonyms", out var antonymsElement) || antonymsElement.ValueKind != JsonValueKind.Array)
            {
                throw new Exception("Antonyms not found or invalid in response.");
            }

            _context.Set(nameof(WordCompletionResponse.Pronunciation), root.GetProperty("pronunciation").GetString() ?? throw new Exception("Pronunciation not found in response."));
            _context.Set(nameof(WordCompletionResponse.PartOfSpeechLabel), root.GetProperty("part_of_speech_free").GetString() ?? throw new Exception("Part of speech label not found in response."));
            _context.Set(nameof(WordCompletionResponse.PartOfSpeechType), parsedPartOfSpeech);
            _context.Set(nameof(WordCompletionResponse.Synonyms), synonymsElement.EnumerateArray().Select(s => s.GetString() ?? string.Empty).ToList());
            _context.Set(nameof(WordCompletionResponse.Antonyms), antonymsElement.EnumerateArray().Select(a => a.GetString() ?? string.Empty).ToList());
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
            "pronunciation": {
              "type": "string",
              "description": "The pronunciation guide for the word, if available."
            },
            "part_of_speech_free": {
              "type": "string",
              "description": "The full, precise grammatical label in free text. E.g. 'phrasal verb', 'modal verb', 'idiomatic expression' in English."
            },
            "part_of_speech_category": {
              "type": "string",
              "enum": [
                "noun",
                "verb",
                "adjective",
                "adverb",
                "pronoun",
                "preposition",
                "conjunction",
                "interjection",
                "article",
                "numeral",
                "phrase",
                "other"
              ],
              "description": "One of the predefined categories for easier filtering and grouping."
            },
            "synonyms": {
              "type": "array",
              "items": {
                "type": "string"
              },
              "description": "A list of synonyms for the word in [SourceLanguage]."
            },
            "antonyms": {
              "type": "array",
              "items": {
                "type": "string"
              },
              "description": "A list of antonyms for the word in [SourceLanguage]."
            }
          },
          "required": ["success", "error_message", "pronunciation", "part_of_speech_free", "part_of_speech_category", "synonyms", "antonyms" ],
          "additionalProperties": false
        }
        """;        
    }
}
