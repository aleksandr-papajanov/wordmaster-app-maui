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
    internal partial class Deck : RealmObject
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;
        public Language SourceLang { get; set; } = null!;
        public Language TargetLang { get; set; } = null!;
        public IList<Word> Words { get; } = null!;
    }
}
