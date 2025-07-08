using WordMaster.Data.Services;
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
        private readonly IWordService _wordService;
        private readonly ISessionBuilder _generator;
        private readonly IWordUsageEntityViewModelFactory _usageFactory;

        public WordUsageViewViewModelFactory(IWordService wordService, ISessionBuilder generator, IWordUsageEntityViewModelFactory usageFactory)
        {
            _wordService = wordService;
            _generator = generator;
            _usageFactory = usageFactory;
        }

        public WordUsageViewViewModel Create(IObservable<WordEntityViewModel?> word)
        {
            return new WordUsageViewViewModel(word, _usageFactory);
        }
    }
}
