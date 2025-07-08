using DynamicData;
using DynamicData.Binding;
using Realms;
using WordMaster.Data.Models;

namespace WordMaster.Data.Services
{
    public interface IDeckService
    {
        IObservable<IChangeSet<Deck>> GetDecksStream();

        Deck? FindByWord(Word word);
    }
    public class DeckService : IDeckService
    {
        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<Deck> _deckRepository;
        private readonly IRepository<WordDeck> _wordDeckRepository;

        public DeckService(IRepository<Deck> deckRepository, IRepository<WordDeck> wordDeckRepository, IRepository<Language> languageRepository)
        {
            _deckRepository = deckRepository;
            _wordDeckRepository = wordDeckRepository;
            _languageRepository = languageRepository;
        }


        public IObservable<IChangeSet<Deck>> GetDecksStream()
        {
            var query = _deckRepository.All;

            return query
                .AsRealmCollection()
                .ToObservableChangeSet<IRealmCollection<Deck>, Deck>();
        }

        public Deck? FindByWord(Word word)
        {
            var found = _wordDeckRepository.All
                .Where(w => w.Word == word)
                .FirstOrDefault();

            if (found == null)
            {
                return null;
            }

            return found.Deck;
        }
    }
}
