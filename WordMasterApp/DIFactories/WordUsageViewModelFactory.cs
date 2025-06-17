using WordMasterApp.EntityWrappers;
using WordMasterApp.Features;
using WordMasterApp.Services;
using WordMasterApp.Services.Generation;

namespace WordMasterApp.DIFactories
{
    public interface IWordUsageViewModelFactory
    {
        WordUsageViewViewModel Create(IObservable<WordWrapper?> word);
    }

    public class WordUsageViewModelFactory : IWordUsageViewModelFactory
    {
        private readonly IWordUsageService _usageService;
        private readonly IGenerationService _aiService;
        private readonly IWordUsageWrapperFactory _usageWrapperFactory;

        public WordUsageViewModelFactory(IWordUsageService usageService, IGenerationService aiService, IWordUsageWrapperFactory wordUsageWrapperFactory)
        {
            _usageService = usageService;
            _aiService = aiService;
            _usageWrapperFactory = wordUsageWrapperFactory;
        }

        public WordUsageViewViewModel Create(IObservable<WordWrapper?> word)
        {
            return new WordUsageViewViewModel(word, _usageService, _aiService, _usageWrapperFactory);
        }
    }
}
