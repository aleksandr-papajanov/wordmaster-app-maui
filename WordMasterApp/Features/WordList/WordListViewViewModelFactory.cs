using WordMasterApp.EntityViewModels;
using WordMasterApp.EntityViewModels.DIFactories;

namespace WordMasterApp.Features.WordList
{
    public interface IWordListViewViewModelFactory
    {
        WordListViewViewModel Create(IObservable<DeckEntityViewModel?> deck);
    }

    public class WordListViewViewModelFactory : IWordListViewViewModelFactory
    {
        private readonly IWordEntityViewModelFactory _factory;

        public WordListViewViewModelFactory(IWordEntityViewModelFactory factory)
        {
            _factory = factory;
        }

        public WordListViewViewModel Create(IObservable<DeckEntityViewModel?> deck)
        {
            return new WordListViewViewModel(deck, _factory);
        }
    }
}
