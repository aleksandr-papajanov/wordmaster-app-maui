using WordMasterApp.DataLayer.Models;
using WordMasterApp.EntityWrappers;
using WordMasterApp.Services;

namespace WordMasterApp.DIFactories
{
    public interface IWordUsageWrapperFactory
    {
        WordUsageWrapper CreateFromEntity(WordUsage entity, IWordUsageService usageService);
        WordUsageWrapper CreateNew(WordWrapper word);
    }

    public class WordUsageWrapperFactory : IWordUsageWrapperFactory
    {
        private readonly Lazy<IWordUsageService> _usageService;

        public WordUsageWrapperFactory(Lazy<IWordUsageService> usageService)
        {
            _usageService = usageService;
        }

        public WordUsageWrapper CreateFromEntity(WordUsage entity, IWordUsageService usageService)
        {
            return new WordUsageWrapper(entity, usageService);
        }

        public WordUsageWrapper CreateNew(WordWrapper word)
        {
            return new WordUsageWrapper(word, _usageService.Value);
        }
    }
}
