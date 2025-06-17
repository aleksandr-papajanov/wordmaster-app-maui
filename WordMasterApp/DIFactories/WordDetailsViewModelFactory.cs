using WordMasterApp.EntityWrappers;
using WordMasterApp.Features;
using WordMasterApp.Features.MessageContainer;
using WordMasterApp.Services;
using WordMasterApp.Services.Generation;

namespace WordMasterApp.DIFactories
{
    public interface IWordDetailsViewModelFactory
    {
        WordDetailsViewModel Create(IObservable<WordWrapper?> word);
    }

    public class WordDetailsViewModelFactory : IWordDetailsViewModelFactory
    {
        private readonly IWordService _wordService;
        private readonly IGenerationService _aiService;
        private readonly IMessageService _messageService;
        private readonly IWordUsageViewModelFactory _usageFactory;

        public WordDetailsViewModelFactory(IWordService service, IGenerationService aiService, IMessageService messageService, IWordUsageViewModelFactory wordUsageFactory)
        {
            _wordService = service;
            _aiService = aiService;
            _messageService = messageService;
            _usageFactory = wordUsageFactory;
        }

        public WordDetailsViewModel Create(IObservable<WordWrapper?> word)
        {
            return new WordDetailsViewModel(word, _wordService, _aiService, _messageService, _usageFactory);
        }
    }
}
