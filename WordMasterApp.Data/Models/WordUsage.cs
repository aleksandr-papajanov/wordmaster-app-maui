using Realms;

namespace WordMasterApp.Data.Models
{
    public class WordUsage : RealmObject
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid WordId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
    }
}
