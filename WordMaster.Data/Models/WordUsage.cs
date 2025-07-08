using Realms;

namespace WordMaster.Data.Models
{
    public partial class WordUsage : RealmObject
    {
        [PrimaryKey]
        public required Guid Id { get; set; }
        public required Word ParentWord { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
    }
}
