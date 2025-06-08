using DynamicData;
using WordMaster.Data.Models;

namespace WordMaster.Data.Services
{
    public interface IWordUsageService
    {
<<<<<<< HEAD
        IObservable<IChangeSet<WordUsage>> GetUsagesStream(IObservable<Guid> wordId, IObservable<string> filter);
=======
        IObservable<IChangeSet<WordUsage>> GetStream(IObservable<Word> word, IObservable<string> filter);

        Task CreateAsync(WordUsage entity);
        Task DeleteAsync(WordUsage entity);
        Task UpdateAsync(WordUsage entity, Action<WordUsage> updater);
>>>>>>> 6c4b5eb (Add word usage feature)
    }
}