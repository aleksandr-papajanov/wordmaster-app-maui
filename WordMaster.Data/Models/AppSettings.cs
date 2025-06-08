using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMaster.Data.Models
{
    public class AppSettings : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; } = "global";
        public bool IsInitialized { get; set; } = false;
    }
}
