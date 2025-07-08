using WordMaster.Common;
using WordMaster.Generation.Interfaces;

namespace WordMaster.Generation.DTOs
{
    public class WordCompletionRequest : IRequest
    {
        private int _count = 1;

        public required string SourceLanguage { get; set; }
        public required string TargetLanguage { get; set; }
        public string Word { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
        public string Definition { get; set; } = string.Empty;
        public required int UsagesCount
        {
            get => _count;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(UsagesCount), "Count must be at least 1.");
                }

                _count = value;
            }
        }

        public void SetToContext(IContext context)
        {
            context.Set(nameof(SourceLanguage), SourceLanguage);
            context.Set(nameof(TargetLanguage), TargetLanguage);
            context.Set(nameof(Word), Word);
            context.Set(nameof(Translation), Translation);
            context.Set(nameof(Definition), Definition);
            context.Set(nameof(UsagesCount), UsagesCount);
        }
    };

    public record WordCompletionResponse
    {
        public required string Word { get; set; }
        public required string Translation { get; set; }
        public required string Definition { get; set; }
        public required string Pronunciation { get; set; }
        public required string PartOfSpeechLabel { get; set; }
        public required PartOfSpeech PartOfSpeechType { get; set; }
        public required IEnumerable<string> Synonyms { get; set; }
        public required IEnumerable<string> Antonyms { get; set; }
        public required WordUsageFrequenсy UsageFrequency { get; set; }
        public required Dictionary<string, string> Sentences { get; set; }
    }

    public record WordBasicsCompletionResponse
    {
        public required string Word { get; set; }
        public required string Translation { get; set; }
        public required string Definition { get; set; }
        public required WordUsageFrequenсy UsageFrequency { get; set; }
    }

    public record WordDetailsCompletionResponse
    {
        public required string Word { get; set; }
        public required string Pronunciation { get; set; }
        public required string PartOfSpeechLabel { get; set; }
        public required PartOfSpeech PartOfSpeechType { get; set; }
        public required IEnumerable<string> Synonyms { get; set; }
        public required IEnumerable<string> Antonyms { get; set; }
    }

    public record WordUsagesCompletionResponse
    {
        public required string Word { get; set; }
        public required Dictionary<string, string> Sentences { get; set; }
    }
}
