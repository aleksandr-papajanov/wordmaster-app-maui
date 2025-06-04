using WordMaster.Data.Services;
using WordMaster.Data.ViewModels;
using WordMasterApp.Features;

namespace WordMasterApp.Infrastructure
{
    public interface IWordUsageViewModelFactory
    {
        WordUsageViewModel Create(IObservable<WordViewModel> stream);
    }

    public class WordUsageViewModelFactory : IWordUsageViewModelFactory
    {
        private readonly IWordUsageService _service;

        public WordUsageViewModelFactory(IWordUsageService service)
        {
            _service = service;
        }

        public WordUsageViewModel Create(IObservable<WordViewModel> stream)
        {
            return new WordUsageViewModel(stream, _service);
        }
    }
}
