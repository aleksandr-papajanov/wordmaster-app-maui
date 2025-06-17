using DynamicData;
using Realms;
using WordMasterApp.DataLayer.Models;
using WordMasterApp.EntityWrappers;

namespace WordMasterApp.Services
{
    public interface IWordUsageService
    {
        IObservable<IChangeSet<WordUsageWrapper>> GetStream(Guid wordId, string filter);

        WordWrapper? GetWord(WordUsage usage);
        Task CreateAsync(WordUsage usage);
        Task DeleteAsync(WordUsage usage);
        Task UpdateAsync(WordUsage usage, Action<Transaction> updater);

    }
}