using WordMasterApp.DataLayer.Models;
using WordMasterApp.EntityWrappers;
using WordMasterApp.Services;

namespace WordMasterApp.DIFactories
{
    public interface IWordWrapperFactory
    {
        WordWrapper CreateFromEntity(Word entity, IWordService wordService);
        WordWrapper CreateNew(DeckWrapper deck);
    }

    public class WordWrapperFactory : IWordWrapperFactory
    {
        private readonly Lazy<IWordService> _lazyWordService;

        public WordWrapperFactory(Lazy<IWordService> lazyWordService)
        {
            _lazyWordService = lazyWordService;
        }

        public WordWrapper CreateFromEntity(Word entity, IWordService wordService)
        {
			return new WordWrapper(entity, wordService);
        }
        
        public WordWrapper CreateNew(DeckWrapper deck)
        {
            return new WordWrapper(deck, _lazyWordService.Value);
        }
    }
}
