using WordMaster.Common;
using WordMaster.Generation.Interfaces;

namespace WordMaster.Generation.DTOs
{
    public static class CompletionResponseMapper
    {
        public static WordCompletionResponse ToWordCompletionResponse(this IContextSnapshot context)
        {
            return new WordCompletionResponse
            {
                Word              = context.Get<string>(nameof(WordCompletionResponse.Word)) ?? throw new ArgumentNullException(nameof(WordCompletionResponse.Word), "Argument not found in context while mapping to word completion response."),
                Translation       = context.Get<string>(nameof(WordCompletionResponse.Translation)) ?? throw new ArgumentNullException(nameof(WordCompletionResponse.Translation), "Argument not found in context while mapping to word completion response."),
                Definition        = context.Get<string>(nameof(WordCompletionResponse.Definition)) ?? throw new ArgumentNullException(nameof(WordCompletionResponse.Definition), "Argument not found in context while mapping to word completion response."),
                Pronunciation     = context.Get<string>(nameof(WordCompletionResponse.Pronunciation)) ?? throw new ArgumentNullException(nameof(WordCompletionResponse.Pronunciation), "Argument not found in context while mapping to word completion response."),
                PartOfSpeechLabel = context.Get<string>(nameof(WordCompletionResponse.PartOfSpeechLabel)) ?? throw new ArgumentNullException(nameof(WordCompletionResponse.PartOfSpeechLabel), "Argument not found in context while mapping to word completion response."),
                PartOfSpeechType  = context.Get<PartOfSpeech>(nameof(WordCompletionResponse.PartOfSpeechType)),
                Synonyms          = context.Get<IEnumerable<string>>(nameof(WordCompletionResponse.Synonyms)) ?? Enumerable.Empty<string>(),
                Antonyms          = context.Get<IEnumerable<string>>(nameof(WordCompletionResponse.Antonyms)) ?? Enumerable.Empty<string>(),
                UsageFrequency    = context.Get<WordUsageFrequenсy>(nameof(WordCompletionResponse.UsageFrequency)),
                Sentences         = context.Get<Dictionary<string, string>>(nameof(WordCompletionResponse.Sentences)) ?? new Dictionary<string, string>()
            };
        }


        public static WordBasicsCompletionResponse ToWordBasicsCompletionResponse(this IContextSnapshot context)
        {
            return new WordBasicsCompletionResponse
            {
                Word           = context.Get<string>(nameof(WordCompletionResponse.Word)) ?? throw new ArgumentNullException(nameof(WordCompletionResponse.Word), "Argument not found in context while mapping to word completion response."),
                Translation    = context.Get<string>(nameof(WordCompletionResponse.Translation)) ?? throw new ArgumentNullException(nameof(WordCompletionResponse.Translation), "Argument not found in context while mapping to word completion response."),
                Definition     = context.Get<string>(nameof(WordCompletionResponse.Definition)) ?? throw new ArgumentNullException(nameof(WordCompletionResponse.Definition), "Argument not found in context while mapping to word completion response."),
                UsageFrequency = context.Get<WordUsageFrequenсy>(nameof(WordCompletionResponse.UsageFrequency)),
            };
        }

        public static WordDetailsCompletionResponse ToWordDetailsCompletionResponse(this IContextSnapshot context)
        {
            return new WordDetailsCompletionResponse
            {
                Word              = context.Get<string>(nameof(WordCompletionResponse.Word)) ?? throw new ArgumentNullException(nameof(WordCompletionResponse.Word), "Argument not found in context while mapping to word completion response."),
                Pronunciation     = context.Get<string>(nameof(WordCompletionResponse.Pronunciation)) ?? throw new ArgumentNullException(nameof(WordCompletionResponse.Pronunciation), "Argument not found in context while mapping to word completion response."),
                PartOfSpeechLabel = context.Get<string>(nameof(WordCompletionResponse.PartOfSpeechLabel)) ?? throw new ArgumentNullException(nameof(WordCompletionResponse.PartOfSpeechLabel), "Argument not found in context while mapping to word completion response."),
                PartOfSpeechType  = context.Get<PartOfSpeech>(nameof(WordCompletionResponse.PartOfSpeechType)),
                Synonyms          = context.Get<IEnumerable<string>>(nameof(WordCompletionResponse.Synonyms)) ?? Enumerable.Empty<string>(),
                Antonyms          = context.Get<IEnumerable<string>>(nameof(WordCompletionResponse.Antonyms)) ?? Enumerable.Empty<string>(),
            };
        }

        public static WordUsagesCompletionResponse ToWordUsagesCompletionResponse(this IContextSnapshot context)
        {
            return new WordUsagesCompletionResponse
            {
                Word      = context.Get<string>(nameof(WordCompletionResponse.Word)) ?? throw new ArgumentNullException(nameof(WordCompletionResponse.Word), "Argument not found in context while mapping to word completion response."),
                Sentences = context.Get<Dictionary<string, string>>(nameof(WordCompletionResponse.Sentences)) ?? new Dictionary<string, string>()
            };
        }
    }
}
