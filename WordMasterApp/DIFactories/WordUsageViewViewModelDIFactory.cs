using WordMaster.Data.Models;
using WordMaster.Data.Services;
using WordMaster.Data.ViewModels;
using WordMasterApp.Features;

namespace WordMasterApp.DIFactories
{
    public interface IWordUsageViewViewModelDIFactory
    {
        WordUsageViewViewModel Create(IObservable<Word?> word);
    }

    public class WordUsageViewViewModelDIFactory : IWordUsageViewViewModelDIFactory
    {
        private readonly IWordUsageService _service;
        private readonly IWordUsageWrapperViewModelDIFactory _wordUsageWrapperFactory;

        public WordUsageViewViewModelDIFactory(IWordUsageService service, IWordUsageWrapperViewModelDIFactory wordUsageWrapperFactory)
        {
            _service = service;
            _wordUsageWrapperFactory = wordUsageWrapperFactory;
        }

        public WordUsageViewViewModel Create(IObservable<Word?> word)
        {
            return new WordUsageViewViewModel(word, _service, _wordUsageWrapperFactory);
        }
    }
}
