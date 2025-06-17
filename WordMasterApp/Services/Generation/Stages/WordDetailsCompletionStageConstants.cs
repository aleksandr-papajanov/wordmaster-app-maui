using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMasterApp.Services.Generation.Stages
{
    internal static class WordDetailsCompletionStageConstants
    {
        public const string ResonseName = "complete_word_details";
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
          "required": ["success", "error_message", "pronunciation", "part_of_speech_free", "part_of_speech_category", "frequency", "synonyms", "antonyms" ],
          "additionalProperties": false
        }
        """;

        public readonly static PromptPair Prompt = new(
            $"Based on this, generate the following additional details:\r\n" +
            $"- Pronunciation guide, if available.\r\n" +
            $"- A clear free-text part of speech label (e.g., 'phrasal verb', 'modal verb', etc.).\r\n" +
            $"- Select the best matching part of speech category from the predefined list.\r\n" +
            $"- Provide an approximate frequency of use from the predefined list.\r\n" +
            $"- Generate a list of common synonyms for this word in [SourceLanguage].\r\n" +
            $"- Generate a list of common antonyms for this word in [SourceLanguage].\r\n",
            "The previous additional details were not satisfactory. " +
            "Please refine the pronunciation, part of speech labels, frequency, synonyms, and antonyms to be more precise and natural."
        );

        internal record PromptPair(string InitialPrompt, string RetryPrompt);
    }
}
