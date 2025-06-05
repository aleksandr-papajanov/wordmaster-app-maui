using WordMaster.Data.Infrastructure;
using WordMaster.Data.Models;
using WordMaster.Data.Services;
using WordMaster.Data.ViewModels;
using WordMasterApp.Features;

namespace WordMasterApp.Infrastructure
{
    public interface IWordUsageServiceFactory
    {
        WordUsageService1 Create(IObservable<Guid> currentWordId);
    }

    public class WordUsageServiceFactory : IWordUsageServiceFactory
    {
        private readonly IRepository<WordUsage> _repository;

        public WordUsageServiceFactory(IRepository<WordUsage> repository)
        {
            _repository = repository;
        }

        public WordUsageService1 Create(IObservable<Guid> currentWordId)
        {
            return new WordUsageService1(currentWordId, _repository);
        }
        
    }
}
