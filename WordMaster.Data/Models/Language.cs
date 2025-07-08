using Realms;

namespace WordMaster.Data.Models
{
    public partial class Language : RealmObject
    {
        [PrimaryKey]
        public required string Code { get; set; }
        public string Name { get; set; } = string.Empty;
        
    }
}
