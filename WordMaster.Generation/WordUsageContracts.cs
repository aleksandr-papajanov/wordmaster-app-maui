using System.Text.Json;

namespace WordMaster.Generation
{
    public class WordUsageGenerationRequest
    {
        public static readonly string _responceSchemaName = "word_usage_generation";
        public static readonly string _responceSchema = """
        {
          "type": "object",
          "properties": {
            "examples": {
              "type": "array",
              "items": {
                "type": "object",
                "properties": {
                  "sentence": {
                    "type": "string",
                    "description": "A naturally sounding sentence using the word."
                  },
                  "translation": {
                    "type": "string",
                    "description": "The translation of the sentence into the target language."
                  }
                },
                "required": ["sentence", "translation"],
                "additionalProperties": false
              }
            },
            "error": {
              "type": "string",
              "description": "An error message if the generation failed."
            }
          },
          "required": ["examples", "error"],
          "additionalProperties": false
        }
        """;

        public string ResponseSchemaName => _responceSchemaName;
        public string ResponseSchema => _responceSchema;

        public string Word { get; }
        public string SourceLanguage { get; }
        public string Translation { get; }
        public string TargetLanguage { get; }
        public int Count { get; }

        public WordUsageGenerationRequest(string word, string sourceLang, string translation, string targetLang, int count)
        {
            Word = word;
            SourceLanguage = sourceLang;
            TargetLanguage = targetLang;
            Translation = translation;
            Count = count;
        }

        public string GetPrompt()
        {
            return $"Generate {Count} naturally sounding sentence in {SourceLanguage} " +
                $"and its translation to {TargetLanguage} " +
                $"using the word \"{Word}\". " +
                $"If you can not do it - state the reason in \"error\" property";
        }
    }

    public class WordUsageGenerationResponse
    {
        public Dictionary<string, string> Examples { get; set; } = new();

        public void ParseResponse(string response)
        {
            using JsonDocument structuredJson = JsonDocument.Parse(response);

            var error = structuredJson.RootElement.GetProperty("error").GetString();

            if (!string.IsNullOrWhiteSpace(error))
            {
                throw new Exception($"Error parsing a result of generation: {error}");
            }

            Examples = structuredJson.RootElement.GetProperty("examples")
                .EnumerateArray()
                .Aggregate(Examples, (dict, item) =>
                {
                    string sentence = item.GetProperty("sentence").GetString() ?? string.Empty;
                    string translation = item.GetProperty("translation").GetString() ?? string.Empty;

                    dict[sentence] = translation;

                    return dict;
                });
        }
    }
}
