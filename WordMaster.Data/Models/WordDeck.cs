using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMaster.Data.Models
{
    public partial class WordDeck : RealmObject
    {
        public required Word Word { get; set; }
        public required Deck Deck { get; set; }
    }
}
