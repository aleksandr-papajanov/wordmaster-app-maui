using DynamicData;
using Realms;
using WordMasterApp.Data.Models;

namespace WordMasterApp.Data.Services.Interfaces
{
    public interface IWordService
    {
        IObservable<IChangeSet<Word>> GetStream(Guid deckId, string filter);

        Word? Find(Guid id);

        Task CreateAsync(Word word);
        Task DeleteAsync(Word word);
        Task UpdateAsync(Word word, Action<Transaction> updater);
    }
}