using DynamicData;
using Realms;
using WordMasterApp.Data.Models;

namespace WordMasterApp.Data.Services.Interfaces
{
    public interface IWordUsageService
    {
        IObservable<IChangeSet<WordUsage>> GetStream(Guid wordId, string filter);

        Task CreateAsync(WordUsage usage);
        Task DeleteAsync(WordUsage usage);
        Task UpdateAsync(WordUsage usage, Action<Transaction> updater);
    }
}