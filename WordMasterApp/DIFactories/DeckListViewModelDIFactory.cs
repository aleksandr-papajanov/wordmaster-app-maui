using WordMaster.Data.Models;
using WordMaster.Data.Services.Interfaces;
using WordMaster.Data.ViewModels;
using WordMasterApp.Features;
using WordMasterApp.Features.DeckList;
using WordMasterApp.Features.WordList;

namespace WordMasterApp.DIFactories
{
    public interface IDeckListViewModelDIFactory
    {
        DeckListViewModel Create();
    }

    public class DeckListViewModelDIFactory : IDeckListViewModelDIFactory
    {
        private readonly IDeckService _service;

        public DeckListViewModelDIFactory(IDeckService service)
        {
            _service = service;
        }

        public DeckListViewModel Create()
        {
            return new DeckListViewModel(_service);
        }
    }
}
