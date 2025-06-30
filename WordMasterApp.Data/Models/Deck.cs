using Realms;

namespace WordMasterApp.Data.Models
{
    public partial class Deck : RealmObject
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string SourceLanguageCode { get; set; } = string.Empty;
        public string TargetLanguageCode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        
    }
}
