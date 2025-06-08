using DynamicData;
using DynamicData.Binding;
using Realms;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WordMaster.Data.Infrastructure;
using WordMaster.Data.Models;

namespace WordMaster.Data.Services
{
    public class WordUsageService : IWordUsageService
    {
        private readonly IRepository<WordUsage> _repository;

        public WordUsageService(IRepository<WordUsage> repository)
        {
            _repository = repository;
        }
        
        public IObservable<IChangeSet<WordUsage>> GetStream(IObservable<Word> word, IObservable<string> filter)
        {
            return word
                .CombineLatest(filter, (word, filter) => (word, filter))
                .Select(args =>
                {
                     var (word, filter) = args;

                     var filteredWordsQuery = _repository.All
                         .Where(e => e.WordId == word.Id)
                         .Where(e => e.Text.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                                     e.Translation.Contains(filter, StringComparison.OrdinalIgnoreCase));

                     return filteredWordsQuery
                        .AsRealmCollection()
                        .ToObservableChangeSet<IRealmCollection<WordUsage>, WordUsage>();
                })
                .Switch();
        }

        public async Task CreateAsync(WordUsage entity)
        {
            entity.Id = Guid.NewGuid(); // Ensure a new ID is generated for the new word

            using (var trans = await _repository.BeginWriteAsync())
            {
                entity.CreatedAt =
                entity.ModifiedAt = DateTimeOffset.UtcNow;

                _repository.Create(entity);

                await trans.CommitAsync();
            }
        }

        public async Task UpdateAsync(WordUsage entity, Action<WordUsage> updater)
        {
            using (var trans = await _repository.BeginWriteAsync())
            {
                updater(entity);

                entity.ModifiedAt = DateTimeOffset.UtcNow;

                _repository.Update(entity);

                await trans.CommitAsync();
            }
        }

        public async Task DeleteAsync(WordUsage entity)
        {
            using (var trans = await _repository.BeginWriteAsync())
            {
                _repository.Delete(entity);

                await trans.CommitAsync();
            }
        }
    }
}
