using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMasterApp.DataLayer.Models;

namespace WordMasterApp.Services.Generation.Stages
{
    public record WordCompletionState(
        string? Word,
        string? Translation,
        string? Definition,
        string? SourceLanguage,
        string? TargetLanguage,
        string? Pronunciation = null,
        string? PartOfSpeechDescription = null,
        PartOfSpeech? PartOfSpeechCategory = null,
        IEnumerable<string>? Synonyms = null,
        IEnumerable<string>? Antonyms = null,
        WordUsageFrequenсy? UsageFrequency = null
    );
}
