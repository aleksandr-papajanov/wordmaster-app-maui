using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMasterApp.Messages
{
    public class WordCreatedMessage
    {
        public Guid NewWordId { get; }

        public WordCreatedMessage(Guid newWordId)
        {
            NewWordId = newWordId;
        }
    }
}
