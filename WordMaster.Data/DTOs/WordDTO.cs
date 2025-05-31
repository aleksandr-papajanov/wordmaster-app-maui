using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMaster.Data.DTOs
{
    public class WordDTO : IDisblayable
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
        public string? Definition { get; set; }
    }
}
