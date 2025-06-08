using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMaster.Data.Models
{
    public class WordUsage : RealmObject
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid WordId { get; set; }

        public string Text { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }

        //public Word? WordId { get; set; }
    }
}
