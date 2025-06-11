using WordMaster.Data.Models;
using WordMaster.Data.Services.Interfaces;
using WordMaster.Data.ViewModels;

namespace WordMasterApp.DIFactories
{
    public interface IWordUsageWrapperViewModelDIFactory
    {
        WordUsageWrapperViewModel Create(WordUsage entity);
        WordUsageWrapperViewModel Create(Word word);
    }

    public class WordUsageWrapperViewModelDIFactory : IWordUsageWrapperViewModelDIFactory
    {
        private readonly IWordUsageService _service;

        public WordUsageWrapperViewModelDIFactory(IWordUsageService service)
        {
            _service = service;
        }

        public WordUsageWrapperViewModel Create(WordUsage entity)
        {
            return new WordUsageWrapperViewModel(entity, _service);
        }
        
        public WordUsageWrapperViewModel Create(Word word)
        {
            return new WordUsageWrapperViewModel(word, _service);
        }
    }
}
