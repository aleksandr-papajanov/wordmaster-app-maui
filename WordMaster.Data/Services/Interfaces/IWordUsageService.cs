using DynamicData;
using Realms;
using WordMaster.Data.Models;

namespace WordMaster.Data.Services.Interfaces
{
    public interface IWordUsageService
    {
        IObservable<IChangeSet<WordUsage>> GetStream(Guid wordId, string filter);

        Task CreateAsync(WordUsage entity);
        Task DeleteAsync(WordUsage entity);
        Task UpdateAsync(WordUsage entity, Action<WordUsage> updater);

    }
}