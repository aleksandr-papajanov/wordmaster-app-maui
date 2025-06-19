using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMasterApp.Services.Generation
{
    public interface IGenerationContextSnapshot
    {
        GenerationSessionResult? SessionResult { get; set; }
        string? ErrorMessage { get; set; }
        T Get<T>(string key);
    }
}
