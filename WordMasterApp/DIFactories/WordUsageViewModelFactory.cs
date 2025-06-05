using WordMaster.Data.Services;
using WordMaster.Data.ViewModels;
using WordMasterApp.Features;

namespace WordMasterApp.DIFactories
{
    public interface IWordUsageViewModelFactory
    {
        WordUsageViewModel Create(IObservable<Guid> wordId);
    }

    public class WordUsageViewModelFactory : IWordUsageViewModelFactory
    {
        private readonly IWordUsageService _service;

        public WordUsageViewModelFactory(IWordUsageService service)
        {
            _service = service;
        }

        public WordUsageViewModel Create(IObservable<Guid> wordId)
        {
            return new WordUsageViewModel(wordId, _service);
        }
    }
}
