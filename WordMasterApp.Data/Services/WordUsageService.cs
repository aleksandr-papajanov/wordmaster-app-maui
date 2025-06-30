using DynamicData;
using DynamicData.Binding;
using Realms;
using System.Reactive.Linq;
using WordMasterApp.Data.Models;
using WordMasterApp.Data.Services.Interfaces;

namespace WordMasterApp.Data.Services
{
    public class WordUsageService : IWordUsageService
    {
        private readonly IRepository<WordUsage> _wordUsageRepository;

        public WordUsageService(IRepository<WordUsage> wordUsageRepository)
        {
            _wordUsageRepository = wordUsageRepository;
        }

        public IObservable<IChangeSet<WordUsage>> GetStream(Guid wordId, string filter)
        {
            var query = _wordUsageRepository.All
                .Where(x => x.WordId == wordId)
                .Where(x => x.Text.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                            x.Translation.Contains(filter, StringComparison.OrdinalIgnoreCase));

            return query
                .AsRealmCollection()
                .ToObservableChangeSet<IRealmCollection<WordUsage>, WordUsage>();
        }

        public Task CreateAsync(WordUsage usage)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(WordUsage usage)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(WordUsage usage, Action<Transaction> updater)
        {
            throw new NotImplementedException();
        }
    }
}
