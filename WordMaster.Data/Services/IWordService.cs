using DynamicData;
using System.Reactive.Subjects;
using WordMaster.Data.Models;

namespace WordMaster.Data.Services
{
    public interface IWordService
    {
        IObservable<IChangeSet<Word>> Words { get; }
        BehaviorSubject<string> FilterSubject { get; }

        Word? Find(Guid id);
        Task CreateAsync(Word entity);
        Task DeleteAsync(Word entity);
        Task UpdateAsync(Word entity, Action<Word> updater);
    }
}