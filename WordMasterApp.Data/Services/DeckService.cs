using DynamicData;
using DynamicData.Binding;
using Realms;
using WordMasterApp.Data.Models;
using WordMasterApp.Data.Services.Interfaces;

namespace WordMasterApp.Data.Services
{
    public class DeckService : IDeckService
    {
        private readonly IRepository<Deck> _deckRepository;

        public DeckService(IRepository<Deck> deckRepository)
        {
            _deckRepository = deckRepository;
        }


        public IObservable<IChangeSet<Deck>> GetStream()
        {
            var query = _deckRepository.All;

            return query
                .AsRealmCollection()
                .ToObservableChangeSet<IRealmCollection<Deck>, Deck>();
        }

        public Deck? Find(Guid id)
        {
            return _deckRepository.Find(id);
        }

        public Task CreateAsync(Deck word)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Deck word)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Deck word, Action<Transaction> updater)
        {
            throw new NotImplementedException();
        }
    }
}
