using DynamicData;
using WordMaster.Data.Services.Interfaces;
using WordMasterApp.EntityViewModels.DIFactories;

namespace WordMasterApp.Features.DeckList
{
    public interface IDeckListViewViewModelFactory
    {
        DeckListViewModel Create();
    }

    public class DeckListViewViewModelFactory : IDeckListViewViewModelFactory
    {
        private readonly IDeckService _deckService;
        private readonly IDeckEntityViewModelFactory _deckFactory;

        public DeckListViewViewModelFactory(IDeckService deckService, IDeckEntityViewModelFactory deckFactory)
        {
            _deckService = deckService;
            _deckFactory = deckFactory;
        }

        public DeckListViewModel Create()
        {
            var stream = _deckService
                .GetStream()
                .Transform(_deckFactory.Create);

            return new DeckListViewModel(stream);
        }
    }
}
