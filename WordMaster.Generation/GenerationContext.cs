using OpenAI.Chat;
using WordMaster.Generation.Interfaces;

namespace WordMaster.Generation
{
    internal class GenerationContext : IGenerationContext
    {
        private readonly Dictionary<string, object?> _storage = new();

        // IGenerationContext implementation
        public ChatClient Chat { get; }
        public List<ChatMessage> History { get; }
        public GenerationSessionResult? SessionResult { get; set; }
        public string? ErrorMessage { get; set; }

        public GenerationContext(ChatClient chat)
        {
            Chat = chat;
            History = new List<ChatMessage>();
        }

        public void Set<T>(string key, T value) => _storage[key] = value;
        public T Get<T>(string key)
        {
            if (!_storage.TryGetValue(key, out var value) || value is not T casted)
            {
                throw new Exception($"{key} not found in context or of wrong type");
            }

            return casted;
        }
    }
}
