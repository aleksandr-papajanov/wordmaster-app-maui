using DynamicData;
using Realms;
using WordMasterApp.Data.Models;

namespace WordMasterApp.Data.Services.Interfaces
{
    public interface IDeckService
    {
        IObservable<IChangeSet<Deck>> GetStream();

        Deck? Find(Guid id);

        Task CreateAsync(Deck word);
        Task DeleteAsync(Deck word);
        Task UpdateAsync(Deck word, Action<Transaction> updater);
    }
}