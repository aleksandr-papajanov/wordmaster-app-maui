using WordMaster.Data.Models;
using WordMaster.Data.Services;
using WordMaster.Data.ViewModels;
using WordMasterApp.Features;

namespace WordMasterApp.DIFactories
{
    public interface IWordWrapperViewModelDIFactory
    {
        WordWrapperViewModel Create(Word entity);
        WordWrapperViewModel Create();
    }

    public class WordWrapperViewModelDIFactory : IWordWrapperViewModelDIFactory
    {
        private readonly IWordService _service;

        public WordWrapperViewModelDIFactory(IWordService service)
        {
            _service = service;
        }

        public WordWrapperViewModel Create(Word entity)
        {
            return new WordWrapperViewModel(entity, _service);
        }
        
        public WordWrapperViewModel Create()
        {
            return new WordWrapperViewModel(_service);
        }
    }
}
