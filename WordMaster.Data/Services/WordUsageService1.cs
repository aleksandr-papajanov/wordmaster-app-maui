using DynamicData;
using DynamicData.Binding;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMaster.Data.Infrastructure;
using WordMaster.Data.Models;

namespace WordMaster.Data.Services
{
    public class WordUsageService1 : IWordUsageService, IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        //private IDisposable? _realmSubscription;
        private readonly IRepository<WordUsage> _repository;

        public IObservable<IChangeSet<WordUsage>> GetUsagesStream(IObservable<Guid> wordId, IObservable<string> filter)
        {
            //return filter
            //    .DistinctUntilChanged()
            //    .Select(filter =>
            //    {
            //        //_realmSubscription?.Dispose();

            //        var filteredWordsQuery = string.IsNullOrWhiteSpace(filter)
            //            ? _repository.All
            //            : _repository.All
            //                .Where(word => word.Text.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
            //                               word.Translation.Contains(filter, StringComparison.OrdinalIgnoreCase));

            //        return filteredWordsQuery.AsRealmCollection().ToObservableChangeSet1();

            //    })
            //    .Switch();

            return null;
        }

        public WordUsageService1(IObservable<Guid> currentWordId, IRepository<WordUsage> repository)
        {
            _repository = repository;
        }

        public void Dispose()
        {
            _disposables.Dispose();
            //_realmSubscription?.Dispose();
        }
    }
}
