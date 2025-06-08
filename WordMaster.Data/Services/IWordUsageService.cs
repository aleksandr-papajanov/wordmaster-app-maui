using DynamicData;
using WordMaster.Data.Models;

namespace WordMaster.Data.Services
{
    public interface IWordUsageService
    {
        IObservable<IChangeSet<WordUsage>> GetStream(IObservable<Word> word, IObservable<string> filter);

        Task CreateAsync(WordUsage entity);
        Task DeleteAsync(WordUsage entity);
        Task UpdateAsync(WordUsage entity, Action<WordUsage> updater);
    }
}