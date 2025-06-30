using Realms;

namespace WordMaster.Data.Models
{
    public class RelatedWord : RealmObject
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid WordId { get; set; }
        public string Text { get; set; } = string.Empty;
        public int RelationType { get; set; }
        public string? Translation { get; set; }
        public string? Definition { get; set; }
        public int? UsageFrequency { get; set; }
        public Guid? LinkedWordId { get; set; }
    }
}
