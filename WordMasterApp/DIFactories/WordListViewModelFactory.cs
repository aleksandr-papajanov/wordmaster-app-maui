using WordMasterApp.EntityWrappers;
using WordMasterApp.Features.WordList;
using WordMasterApp.Services;

namespace WordMasterApp.DIFactories
{
    public interface IWordListViewModelFactory
    {
        WordListViewViewModel Create(IObservable<DeckWrapper?> deck);
    }

    public class WordListViewModelFactory : IWordListViewModelFactory
    {
        private readonly IWordService _wordService;
        private readonly IWordWrapperFactory _wordWrapperFactory;

        public WordListViewModelFactory(IWordService service, IWordWrapperFactory wordWrapperFactory)
        {
            _wordService = service;
            _wordWrapperFactory = wordWrapperFactory;
        }

        public WordListViewViewModel Create(IObservable<DeckWrapper?> deck)
        {
            return new WordListViewViewModel(deck, _wordService, _wordWrapperFactory);
        }
    }
}
