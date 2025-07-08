using OpenAI.Chat;
using WordMaster.Generation.Interfaces;

namespace WordMaster.Generation
{
    internal class Context : IContext
    {
        private readonly Dictionary<string, object?> _storage = new();

        // IGenerationContext implementation
        public ChatClient Chat { get; }
        public List<ChatMessage> History { get; }
        public SessionResult? SessionResult { get; set; }
        public string? ErrorMessage { get; set; }

        public Context(ChatClient chat)
        {
            Chat = chat;
            History = new List<ChatMessage>();
        }

        public void Set<T>(string key, T value) => _storage[key] = value;
        public T? Get<T>(string key)
        {
            if (_storage.TryGetValue(key, out var value) && value is T casted)
            {
                return casted;
            }

            return default;
        }
    }
}
