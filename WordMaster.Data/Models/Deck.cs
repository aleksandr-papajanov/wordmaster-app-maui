using Realms;
using Realms.Schema;
using Realms.Weaving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMaster.Data.Models
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
