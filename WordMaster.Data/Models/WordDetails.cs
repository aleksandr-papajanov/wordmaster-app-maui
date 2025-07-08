using Realms;
using WordMaster.Common;

namespace WordMaster.Data.Models
{
    public partial class WordDetails : RealmObject
    {
        [PrimaryKey]
        public required Guid Id { get; set; }
        public string Pronunciation { get; set; } = string.Empty;
        public string PartOfSpeechLabel { get; set; } = string.Empty;
        public int PartOfSpeechType { get; set; }

        [Backlink(nameof(Word.Details))]
        public IQueryable<Word> Words { get; } = null!;
    }
}
