using WordMaster.Data.Services.Interfaces;
using WordMaster.Generation.Interfaces;
using WordMasterApp.EntityViewModels;
using WordMasterApp.EntityViewModels.DIFactories;

namespace WordMasterApp.Features.WordUsage
{
    public interface IWordUsageViewViewModelFactory
    {
        WordUsageViewViewModel Create(IObservable<WordEntityViewModel?> word);
    }

    public class WordUsageViewViewModelFactory : IWordUsageViewViewModelFactory
    {
        private readonly IWordUsageService _usageService;
        private readonly IGenerationService _aiService;
        private readonly IWordUsageEntityViewModelFactory _usageFactory;

        public WordUsageViewViewModelFactory(IWordUsageService usageService, IGenerationService aiService, IWordUsageEntityViewModelFactory usageFactory)
        {
            _usageService = usageService;
            _aiService = aiService;
            _usageFactory = usageFactory;
        }

        public WordUsageViewViewModel Create(IObservable<WordEntityViewModel?> word)
        {
            return new WordUsageViewViewModel(word, _usageFactory);
        }
    }
}
