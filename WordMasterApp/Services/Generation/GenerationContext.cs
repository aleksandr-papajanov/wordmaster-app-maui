using OpenAI.Chat;
using System;
using System.Collections.Generic;
using WordMasterApp.DataLayer.Models;

namespace WordMasterApp.Services.Generation
{
    internal class GenerationContext : IGenerationContext
    {
        private readonly Dictionary<string, object?> _storage = new();

        // IGenerationContext implementation
        public ChatClient Chat { get; }
        public List<ChatMessage> History { get; }
        public bool IsComplete { get; set; }

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
