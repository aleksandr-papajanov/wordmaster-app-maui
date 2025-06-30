using Realms;

namespace WordMaster.Data.Models
{
    public class Language : RealmObject
    {
        [PrimaryKey]
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        
    }
}
