using WordMaster.Common;

namespace WordMaster.Generation.DTOs
{
    public record WordCompletionRequest(
        string? Word = null,
        string? Translation = null,
        string? Definition = null,
        string? SourceLanguage = null,
        string? TargetLanguage = null,
        string? Pronunciation = null,
        string? PartOfSpeechDescription = null,
        PartOfSpeech? PartOfSpeechCategory = null,
        IEnumerable<string>? Synonyms = null,
        IEnumerable<string>? Antonyms = null,
        WordUsageFrequenсy? UsageFrequency = null
    );
}
