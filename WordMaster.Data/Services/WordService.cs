using DynamicData;
using DynamicData.Binding;
using Realms;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WordMaster.Data.Infrastructure;
using WordMaster.Data.Models;
using WordMaster.Data.Services.Interfaces;

namespace WordMaster.Data.Services
{
    public class WordService : IWordService
    {
        private readonly IRepository<Word> _repository;

        public WordService(IRepository<Word> repository)
        {
            _repository = repository;
        }

        public IObservable<IChangeSet<Word>> GetStream(Guid deckId, string filter)
        {
            var filteredWordsQuery = _repository.All
                .Where(e => e.DeckId == deckId)
                .Where(e => e.Text.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                            e.Translation.Contains(filter, StringComparison.OrdinalIgnoreCase));

            return filteredWordsQuery
                .AsRealmCollection()
                .ToObservableChangeSet<IRealmCollection<Word>, Word>()
                .Transform(x => x);
        }

        public Word? Find(Guid id)
        {
            return _repository.Find(id);
        }

        public async Task CreateAsync(Word entity)
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

        public async Task UpdateAsync(Word entity, Action<Word> updater)
        {
            using (var trans = await _repository.BeginWriteAsync())
            {
                updater(entity);

                entity.ModifiedAt = DateTimeOffset.UtcNow;

                _repository.Update(entity);

                await trans.CommitAsync();
            }
        }

        public async Task DeleteAsync(Word entity)
        {
            using (var trans = await _repository.BeginWriteAsync())
            {
                _repository.Delete(entity);

                await trans.CommitAsync();
            }
        }

        public void Dispose()
        {
            //_disposables.Dispose();
            //_realmSubscription?.Dispose();
        }
    }
}
