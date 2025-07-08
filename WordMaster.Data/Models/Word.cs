using Realms;

namespace WordMaster.Data.Models
{
    public partial class Word : RealmObject
    {
        [PrimaryKey]
        public required Guid Id { get; set; }
        public string? Text { get; set; }
        public string? Translation { get; set; }
        public string? Definition { get; set; }
        public int? UsageFrequency { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public WordDetails? Details { get; set; }


        [Backlink(nameof(WordDeck.Word))]
        public IQueryable<WordDeck> WordDecks { get; } = null!;

        [Backlink(nameof(WordRelation.ParentWord))]
        public IQueryable<WordRelation> Relations { get; } = null!;

        [Backlink(nameof(WordUsage.ParentWord))]
        public IQueryable<WordUsage> Usages { get; } = null!;
    }
}
