using DynamicData;
using DynamicData.Binding;
using Realms;
using System.Reactive.Linq;
using WordMasterApp.DataLayer;
using WordMasterApp.DataLayer.Models;
using WordMasterApp.DIFactories;
using WordMasterApp.EntityWrappers;
using WordMasterApp.Services.Generation;
using WordMasterApp.Services.Generation.Stages;

namespace WordMasterApp.Services
{
    public class WordService : IWordService
    {
        private readonly IRepository<Word> _wordRepository;
        private readonly IWordWrapperFactory _wordWrapperFactory;
        private readonly IDeckService _deckService;
        private readonly IGenerationService _aiService;

        public WordService(IRepository<Word> repository, IWordWrapperFactory wordWrapperFactory, IDeckService deckService, IGenerationService aiService)
        {
            _wordRepository = repository;
            _wordWrapperFactory = wordWrapperFactory;
            _deckService = deckService;
            _aiService = aiService;
        }

        public IObservable<IChangeSet<WordWrapper>> GetStream(Guid deckId, string filter)
        {
            var query = _wordRepository.All
                .Where(e => e.DeckId == deckId)
                .Where(e => e.Text.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                            e.Translation.Contains(filter, StringComparison.OrdinalIgnoreCase));

            return query
                .AsRealmCollection()
                .ToObservableChangeSet<IRealmCollection<Word>, Word>()
                .Transform(x => _wordWrapperFactory.CreateFromEntity(x, this));
        }


        public DeckWrapper? GetDeck(Word word)
        {
            if (word.DeckId == Guid.Empty || !word.IsManaged)
            {
                throw new ArgumentException("Invalid WordUsage entity provided.");
            }

            return _deckService.Find(word.DeckId);
        }

        public WordWrapper? Find(Guid id)
        {
            var word = _wordRepository.Find(id);

            if (word == null)
            {
                return null;
            }

            return _wordWrapperFactory.CreateFromEntity(word, this);
        }

        public async Task CreateAsync(Word entity)
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedAt =
            entity.ModifiedAt = DateTimeOffset.UtcNow;

            await _wordRepository.CreateAsync(entity);
        }

        public async Task UpdateAsync(Word entity, Action<Transaction> updater)
        {
            await _wordRepository.UpdateAsync(entity, (e, t) =>
            {
                updater(t);

                entity.ModifiedAt = DateTimeOffset.UtcNow;
            });
        }

        public async Task DeleteAsync(Word entity)
        {
            await _wordRepository.DeleteAsync(entity);
        }

        public IGenerationSession AutoComplete(WordWrapper word)
        {
            return _aiService.CompleteWordDetails(
                new WordCompletionState(
                    Word: word.Text,
                    Translation: word.Translation,
                    Definition: word.Definition,
                    SourceLanguage: "Swedish",
                    TargetLanguage: "Russion"
                ));

        }
    }
}
