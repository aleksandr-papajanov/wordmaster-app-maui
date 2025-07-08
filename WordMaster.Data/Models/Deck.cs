using Realms;

namespace WordMaster.Data.Models
{
    public partial class Deck : RealmObject
    {
        [PrimaryKey]
        public required Guid Id { get; set; }
        public required Language SourceLanguage { get; set; }
        public required Language TargetLanguage { get; set; }
        public required string Name { get; set; }

        [Backlink(nameof(WordDeck.Deck))]
        public IQueryable<WordDeck> WordDecks { get; } = null!;
    }
}
