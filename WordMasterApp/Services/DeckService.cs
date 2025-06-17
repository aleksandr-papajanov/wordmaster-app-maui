using DynamicData;
using DynamicData.Binding;
using Realms;
using WordMasterApp.DataLayer;
using WordMasterApp.DataLayer.Models;
using WordMasterApp.DIFactories;
using WordMasterApp.EntityWrappers;

namespace WordMasterApp.Services
{
    public class DeckService : IDeckService
    {
        private readonly IRepository<Deck> _deckRepository;
        private readonly IDeckWrapperFactory _deckWrapperFactory;

        public DeckService(IRepository<Deck> repository, IDeckWrapperFactory deckWrapperFactory)
        {
            _deckRepository = repository;
            _deckWrapperFactory = deckWrapperFactory;
        }
        
        public IObservable<IChangeSet<DeckWrapper>> GetStream()
        {
            var query = _deckRepository.All;

            return query
                .AsRealmCollection()
                .ToObservableChangeSet<IRealmCollection<Deck>, Deck>()
                .Transform(x => _deckWrapperFactory.CreateFromEntity(x, this));
        }

        public DeckWrapper? Find(Guid id)
        {
            var word = _deckRepository.Find(id);

            if (word == null)
            {
                return null;
            }

            return _deckWrapperFactory.CreateFromEntity(word, this);
        }
    }
}
