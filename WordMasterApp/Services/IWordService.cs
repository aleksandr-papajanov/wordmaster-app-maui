using DynamicData;
using Realms;
using WordMasterApp.DataLayer.Models;
using WordMasterApp.EntityWrappers;
using WordMasterApp.Services.Generation;

namespace WordMasterApp.Services
{
    public interface IWordService
    {
        IObservable<IChangeSet<WordWrapper>> GetStream(Guid deckId, string filter);

        DeckWrapper? GetDeck(Word word);
        WordWrapper? Find(Guid id);
        Task CreateAsync(Word word);
        Task DeleteAsync(Word word);
        Task UpdateAsync(Word word, Action<Transaction> updater);
        IGenerationSession AutoComplete(WordWrapper word);
    }
}