using WordMaster.Data.Models;
using WordMaster.Data.Services;
using WordMaster.Data.Services.Interfaces;
using WordMaster.Data.ViewModels;
using WordMasterApp.EntityWrappers;
using WordMasterApp.Features;
using WordMasterApp.Features.WordList;

namespace WordMasterApp.DIFactories
{
    public interface IWordListViewModelDIFactory
    {
        WordListViewViewModel Create(IObservable<DeckWrapperViewModel?> deck);
    }

    public class WordListViewModelDIFactory : IWordListViewModelDIFactory
    {
        private readonly IWordService _service;
        private readonly IWordWrapperViewModelDIFactory _wordWrapperFactory;

        public WordListViewModelDIFactory(IWordService service, IWordWrapperViewModelDIFactory wordWrapperFactory)
        {
            _service = service;
            _wordWrapperFactory = wordWrapperFactory;
        }

        public WordListViewViewModel Create(IObservable<DeckWrapperViewModel?> deck)
        {
            return new WordListViewViewModel(deck, _service, _wordWrapperFactory);
        }
    }
}
