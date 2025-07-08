using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMaster.Generation;

namespace WordMaster.Generation.Interfaces
{
    public interface IContextSnapshot
    {
        SessionResult? SessionResult { get; set; }
        string? ErrorMessage { get; set; }
        T? Get<T>(string key);
    }
}
