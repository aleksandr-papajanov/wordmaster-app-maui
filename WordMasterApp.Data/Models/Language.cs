using Realms;

namespace WordMasterApp.Data.Models
{
    public class Language : RealmObject
    {
        [PrimaryKey]
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        
    }
}
