using WordMasterApp.Features.DeckList;
using WordMasterApp.Services;

namespace WordMasterApp.DIFactories
{
    public interface IDeckListViewModelFactory
    {
        DeckListViewModel Create();
    }

    public class DeckListViewModelFactory : IDeckListViewModelFactory
    {
        private readonly IDeckService _deckService;

        public DeckListViewModelFactory(IDeckService service)
        {
            _deckService = service;
        }

        public DeckListViewModel Create()
        {
            return new DeckListViewModel(_deckService);
        }
    }
}
