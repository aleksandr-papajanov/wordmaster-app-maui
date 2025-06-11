using DynamicData;
using System.Reactive.Subjects;
using WordMaster.Data.Models;

namespace WordMaster.Data.Services.Interfaces
{
    public interface IWordService
    {
        IObservable<IChangeSet<Word>> GetStream(Guid deckId, string filter);

        Task CreateAsync(Word entity);
        Task DeleteAsync(Word entity);
        Task UpdateAsync(Word entity, Action<Word> updater);
    }
}