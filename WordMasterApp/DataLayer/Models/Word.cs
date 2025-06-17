using Realms;

namespace WordMasterApp.DataLayer.Models
{
    public class Word : RealmObject
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid DeckId { get; set; }

        public string Text { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
        public string? Definition { get; set; }
        public string? Pronunciation { get; set; }
        public string? PartOfSpeechLabel { get; set; }
        public int PartOfSpeechType { get; set; }
        public string? Synonyms { get; set; }
        public string? Antonyms { get; set; }
        public int UsageFrequency { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
    }
}
