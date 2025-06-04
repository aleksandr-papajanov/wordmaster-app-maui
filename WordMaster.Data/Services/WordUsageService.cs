using DynamicData;
using Realms;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WordMaster.Data.Infrastructure;
using WordMaster.Data.Models;

namespace WordMaster.Data.Services
{
    public class WordService : IWordService, IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        private IDisposable? _realmSubscription;

        private readonly IRepository<Word> _repository;
        private readonly SourceList<Word> _source = new();

        public IObservable<IChangeSet<Word>> Words => _source.Connect();

        public BehaviorSubject<string> FilterSubject { get; } = new(string.Empty);

        public WordService(IRepository<Word> repository)
        {
            _repository = repository;

            FilterSubject
                .DistinctUntilChanged()
                .Subscribe(UpdateFilteredWords)
                .DisposeWith(_disposables);
        }

        private void UpdateFilteredWords(string filter)
        {
            _realmSubscription?.Dispose();

            var filteredWordsQuery = string.IsNullOrWhiteSpace(filter)
                ? _repository.All
                : _repository.All
                    .Where(word => word.Text.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                                   word.Translation.Contains(filter, StringComparison.OrdinalIgnoreCase));


            var collection = filteredWordsQuery.AsRealmCollection();
            _realmSubscription = collection.BindToSourceList(_source);

            _source.Edit(list =>
            {
                list.Clear();
                list.AddRange(collection.ToList());
            });
        }

        public Word? Find(Guid id)
        {
            return _repository.Find(id);
        }

        public async Task CreateAsync(Word entity)
        {
            entity.Id = Guid.NewGuid(); // Ensure a new ID is generated for the new word

            using (var trans = await _repository.BeginWriteAsync())
            {
                entity.CreatedAt =
                entity.ModifiedAt = DateTimeOffset.UtcNow;

                _repository.Create(entity);

                await trans.CommitAsync();
            }
        }

        public async Task UpdateAsync(Word entity, Action<Word> updater)
        {
            using (var trans = await _repository.BeginWriteAsync())
            {
                updater(entity);

                entity.ModifiedAt = DateTimeOffset.UtcNow;

                _repository.Update(entity);

                await trans.CommitAsync();
            }
        }

        public async Task DeleteAsync(Word entity)
        {
            using (var trans = await _repository.BeginWriteAsync())
            {
                _repository.Delete(entity);

                await trans.CommitAsync();
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _realmSubscription?.Dispose();
        }
    }
}
