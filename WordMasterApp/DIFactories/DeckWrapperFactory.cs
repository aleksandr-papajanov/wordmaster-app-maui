using WordMasterApp.DataLayer.Models;
using WordMasterApp.EntityWrappers;
using WordMasterApp.Services;

namespace WordMasterApp.DIFactories
{
    public interface IDeckWrapperFactory
    {
        DeckWrapper CreateFromEntity(Deck entity, IDeckService deckService);
    }

    public class DeckWrapperFactory : IDeckWrapperFactory
    {
        private readonly Lazy<IDeckService> _lazyDeckService;

        public DeckWrapperFactory(Lazy<IDeckService> lazyDeckService)
        {
            _lazyDeckService = lazyDeckService;
        }

        public DeckWrapper CreateFromEntity(Deck entity, IDeckService deckService)
        {
			return new DeckWrapper(entity, deckService);
        }
    }
}
