using DynamicData;
using WordMaster.Data.Models;

namespace WordMaster.Data.Services
{
    public interface IWordUsageService
    {
        IObservable<IChangeSet<WordUsage>> GetUsagesStream(IObservable<Guid> wordId, IObservable<string> filter);
    }
}