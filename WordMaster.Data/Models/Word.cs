using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMaster.Data.Models
{
    internal class Word : RealmObject
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Text { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
        public string Definition { get; set; } = string.Empty;
    }
}
