using Realms;

namespace WordMasterApp.Data.Models
{
    public class AppSettings : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; } = "global";
        public bool IsInitialized { get; set; } = false;
    }
}
