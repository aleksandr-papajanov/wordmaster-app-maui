using DynamicData;
using DynamicData.Binding;
using Realms;
using System.Reactive.Linq;
using WordMasterApp.DataLayer;
using WordMasterApp.DataLayer.Models;
using WordMasterApp.DIFactories;
using WordMasterApp.EntityWrappers;

namespace WordMasterApp.Services
{
    public class WordUsageService : IWordUsageService
    {
        private readonly IRepository<WordUsage> _usageRepository;
        private readonly IWordService _wordService;
        private readonly IWordUsageWrapperFactory _usageWrapperFactory;

        public WordUsageService(IRepository<WordUsage> wordRepository, IWordUsageWrapperFactory wordUsageWrapperFactory, IWordService wordService)
        {
            _usageRepository = wordRepository;
            _usageWrapperFactory = wordUsageWrapperFactory;
            _wordService = wordService;
        }

        public IObservable<IChangeSet<WordUsageWrapper>> GetStream(Guid wordId, string filter)
        {
            var query = _usageRepository.All
                .Where(e => e.WordId == wordId)
                .Where(e => e.Text.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                            e.Translation.Contains(filter, StringComparison.OrdinalIgnoreCase));

            return query
                .AsRealmCollection()
                .ToObservableChangeSet<IRealmCollection<WordUsage>, WordUsage>()
                .Transform(x => _usageWrapperFactory.CreateFromEntity(x, this));
        }


        public WordWrapper? GetWord(WordUsage usage)
        {
            if (usage.WordId == Guid.Empty || !usage.IsManaged)
            {
                throw new ArgumentException("Invalid WordUsage entity provided.");
            }

            return _wordService.Find(usage.WordId);
        }

        public async Task CreateAsync(WordUsage entity)
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedAt =
            entity.ModifiedAt = DateTimeOffset.UtcNow;

            await _usageRepository.CreateAsync(entity);
        }

        public async Task UpdateAsync(WordUsage entity, Action<Transaction> updater)
        {
            await _usageRepository.UpdateAsync(entity, (e, t) =>
            {
                updater(t);

                entity.ModifiedAt = DateTimeOffset.UtcNow;
            });
        }

        public async Task DeleteAsync(WordUsage entity)
        {
            await _usageRepository.DeleteAsync(entity);
        }
    }
}
