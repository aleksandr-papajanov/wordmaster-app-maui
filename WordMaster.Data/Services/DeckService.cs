using DynamicData;
using DynamicData.Binding;
using Realms;
using WordMaster.Data.Infrastructure;
using WordMaster.Data.Models;
using WordMaster.Data.Services.Interfaces;

namespace WordMaster.Data.Services
{
    public class DeckService : IDeckService
    {
        private readonly IRepository<Deck> _repository;

        public DeckService(IRepository<Deck> repository)
        {
            _repository = repository;
        }
        
        public IObservable<IChangeSet<Deck>> GetChangeSet()
        {
            var query = _repository.All;

            return query
                .AsRealmCollection()
                .ToObservableChangeSet<IRealmCollection<Deck>, Deck>();
        }

        //public async Task CreateAsync(WordUsage entity)
        //{
        //    entity.Id = Guid.NewGuid(); // Ensure a new ID is generated for the new word

        //    using (var trans = await _repository.BeginWriteAsync())
        //    {
        //        entity.CreatedAt =
        //        entity.ModifiedAt = DateTimeOffset.UtcNow;

        //        _repository.Create(entity);

        //        await trans.CommitAsync();
        //    }
        //}

        //public async Task UpdateAsync(WordUsage entity, Action<WordUsage> updater)
        //{
        //    using (var trans = await _repository.BeginWriteAsync())
        //    {
        //        updater(entity);

        //        entity.ModifiedAt = DateTimeOffset.UtcNow;

        //        _repository.Update(entity);

        //        await trans.CommitAsync();
        //    }
        //}

        //public async Task DeleteAsync(WordUsage entity)
        //{
        //    using (var trans = await _repository.BeginWriteAsync())
        //    {
        //        _repository.Delete(entity);

        //        await trans.CommitAsync();
        //    }
        //}
    }
}
