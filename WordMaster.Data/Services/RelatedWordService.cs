using DynamicData;
using DynamicData.Binding;
using Realms;
using System.Reactive.Linq;
using WordMaster.Common;
using WordMaster.Data.Models;
using WordMaster.Data.Services.Interfaces;

namespace WordMaster.Data.Services
{
    public class RelatedWordService : IRelatedWordService
    {
        private readonly IRepository<RelatedWord> _relatedWordRepository;

        public RelatedWordService(IRepository<RelatedWord> relatedWordRepository)
        {
            _relatedWordRepository = relatedWordRepository;
        }


        public IObservable<IChangeSet<RelatedWord>> GetStream(Guid wordId, WordRelationType relationType)
        {
            var query = _relatedWordRepository.All
               .Where(x => x.WordId == wordId)
               .Where(x => x.RelationType == (int)relationType);

            return query
                .AsRealmCollection()
                .ToObservableChangeSet<IRealmCollection<RelatedWord>, RelatedWord>();
        }

        public Task CreateAsync(RelatedWord relatedWord)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(RelatedWord relatedWord)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(RelatedWord relatedWord, Action<Transaction> updater)
        {
            throw new NotImplementedException();
        }
    }
}
