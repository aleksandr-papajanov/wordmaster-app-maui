using DynamicData;
using Realms;
using WordMaster.Common;
using WordMaster.Data.Models;

namespace WordMaster.Data.Services.Interfaces
{
    public interface IRelatedWordService
    {
        IObservable<IChangeSet<RelatedWord>> GetStream(Guid wordId, WordRelationType relationType);

        Task CreateAsync(RelatedWord relatedWord);
        Task DeleteAsync(RelatedWord relatedWord);
        Task UpdateAsync(RelatedWord relatedWord, Action<Transaction> updater);
    }
}