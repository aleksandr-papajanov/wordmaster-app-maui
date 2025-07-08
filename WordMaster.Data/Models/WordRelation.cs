using Realms;

namespace WordMaster.Data.Models
{
    public partial class WordRelation : RealmObject
    {
        [PrimaryKey]
        public required Guid Id { get; set; }
        public required Word ParentWord { get; set; }
        public required Word RelatedWord { get; set; }
        public int RelationType { get; set; }
        public bool IsCompleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
    }
}
