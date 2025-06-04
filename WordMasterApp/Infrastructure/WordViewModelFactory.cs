using WordMaster.Data.Models;
using WordMaster.Data.Services;
using WordMaster.Data.ViewModels;
using WordMasterApp.Features;

namespace WordMasterApp.Infrastructure
{
    public interface IWordViewModelFactory
    {
        WordViewModel Create(Word entity);
        WordViewModel Create();
    }

    public class WordViewModelFactory : IWordViewModelFactory
    {
        private readonly IWordService _service;

        public WordViewModelFactory(IWordService service)
        {
            _service = service;
        }

        public WordViewModel Create(Word entity)
        {
            return new WordViewModel(entity, _service);
        }
        
        public WordViewModel Create()
        {
            return new WordViewModel(_service);
        }
    }
}
