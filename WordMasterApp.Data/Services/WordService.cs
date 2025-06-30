using DynamicData;
using DynamicData.Binding;
using Realms;
using System.Reactive.Linq;
using WordMasterApp.Data.Models;
using WordMasterApp.Data.Services.Interfaces;

namespace WordMasterApp.Data.Services
{
    public class WordService : IWordService
    {
        private readonly IRepository<Word> _wordRepository;

        public WordService(IRepository<Word> wordRepository)
        {
            _wordRepository = wordRepository;
        }


        public IObservable<IChangeSet<Word>> GetStream(Guid deckId, string filter)
        {
            var query = _wordRepository.All
                .Where(x => x.DeckId == deckId)
                .Where(x => x.Text.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                            x.Translation.Contains(filter, StringComparison.OrdinalIgnoreCase));

            return query
                .AsRealmCollection()
                .ToObservableChangeSet<IRealmCollection<Word>, Word>();
        }

        public Word? Find(Guid id)
        {
            return _wordRepository.Find(id);
        }

        public async Task CreateAsync(Word word)
        {
            word.Id = Guid.NewGuid();
            word.CreatedAt =
            word.ModifiedAt = DateTimeOffset.UtcNow;

            await _wordRepository.CreateAsync(word);
        }

        public async Task DeleteAsync(Word word)
        {
            await _wordRepository.DeleteAsync(word);
        }

        public async Task UpdateAsync(Word word, Action<Transaction> updater)
        {
            await _wordRepository.UpdateAsync(word, transaction =>
            {
                updater(transaction);

                word.ModifiedAt = DateTimeOffset.UtcNow;
            });
        }
    }
}
