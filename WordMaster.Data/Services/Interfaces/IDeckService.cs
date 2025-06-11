using DynamicData;
using Realms;
using WordMaster.Data.Models;

namespace WordMaster.Data.Services.Interfaces
{
    public interface IDeckService
    {
        IObservable<IChangeSet<Deck>> GetChangeSet();

        //Task CreateAsync(WordUsage entity);
        //Task DeleteAsync(WordUsage entity);
        //Task UpdateAsync(WordUsage entity, Action<WordUsage> updater);

    }
}