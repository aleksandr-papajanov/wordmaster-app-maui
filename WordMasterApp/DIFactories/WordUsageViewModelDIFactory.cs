using WordMaster.Data.Models;
using WordMaster.Data.Services.Interfaces;
using WordMaster.Data.ViewModels;
using WordMasterApp.Features;

namespace WordMasterApp.DIFactories
{
    public interface IWordUsageViewModelDIFactory
    {
        WordUsageViewViewModel Create(IObservable<WordWrapperViewModel?> word);
    }

    public class WordUsageViewModelDIFactory : IWordUsageViewModelDIFactory
    {
        private readonly IWordUsageService _service;
        private readonly IWordUsageWrapperViewModelDIFactory _wordUsageWrapperFactory;

        public WordUsageViewModelDIFactory(IWordUsageService service, IWordUsageWrapperViewModelDIFactory wordUsageWrapperFactory)
        {
            _service = service;
            _wordUsageWrapperFactory = wordUsageWrapperFactory;
        }

        public WordUsageViewViewModel Create(IObservable<WordWrapperViewModel?> word)
        {
            return new WordUsageViewViewModel(word, _service, _wordUsageWrapperFactory);
        }
    }
}
