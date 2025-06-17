using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMasterApp.Services.Generation
{
    public interface IGenerationContext : IGenerationContextSnapshot
    {
        ChatClient Chat { get; }
        List<ChatMessage> History { get; }
        void Set<T>(string key, T value);
    }
}
