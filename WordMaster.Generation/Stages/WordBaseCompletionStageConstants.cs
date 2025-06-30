using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMaster.Generation.Stages
{
    internal static class WordBaseCompletionStageConstants
    {
        public const string ResonseName = "complete_word";
        public const string ResonseSchema = """
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

        internal record PromptPair(string InitialPrompt, string RetryPrompt);
    }
}
