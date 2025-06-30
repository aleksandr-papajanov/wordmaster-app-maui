using DynamicData;
using Realms;
using WordMaster.Data.Models;

namespace WordMaster.Data.Services.Interfaces
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