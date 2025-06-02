using DynamicData;
using Realms;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WordMaster.Data.DTOs;
using WordMaster.Data.DTOs.Mappers;
using WordMaster.Data.Infrastructure.Interfaces;
using WordMaster.Data.Models;

namespace WordMaster.Data.Services
{
    public class WordService : IWordService, IDisposable
    {
        private IDisposable _realmSubscription;

        private readonly IRepository<Word> _wordRepository;

        public SourceList<WordDTO> WordsSource { get; } = new();

        public BehaviorSubject<string> FilterSubject { get; } = new(string.Empty);

        

        public WordService(IRepository<Word> wordRepository)
        {
            _wordRepository = wordRepository;

            _realmSubscription = _wordRepository.All
                .AsRealmCollection()
                .SubscribeForNotifications(OnRealmNotified);

            FilterSubject.Subscribe(filterText =>
            {
                _realmSubscription?.Dispose();

                var filteredWordsQuery = string.IsNullOrWhiteSpace(filterText)
                    ? _wordRepository.All
                    : _wordRepository.All
                        .Where(word => word.Text.Contains(filterText, StringComparison.OrdinalIgnoreCase) ||
                               word.Translation.Contains(filterText, StringComparison.OrdinalIgnoreCase) ||
                               word.Definition.Contains(filterText, StringComparison.OrdinalIgnoreCase));

                var _realmCollection = filteredWordsQuery.AsRealmCollection();

                _realmSubscription = _realmCollection.SubscribeForNotifications(OnRealmNotified);

                var result = _realmCollection
                    .Select(word => word.ToDTO())
                    .ToList();

                WordsSource.Edit(list =>
                {
                    list.Clear();
                    list.AddRange(result);
                });
            });
        }

        private void OnRealmNotified(IRealmCollection<Word> sender, ChangeSet? changes)
        {
            if (changes == null)
            {
                return;
            }

            WordsSource.Edit(sourceList =>
            {
                foreach (var index in changes.DeletedIndices)
                {
                    sourceList.RemoveAt(index);
                }

                foreach (var index in changes.ModifiedIndices)
                {
                    sourceList[index] = sender[index].ToDTO();
                }

                foreach (var index in changes.InsertedIndices)
                {
                    sourceList.Insert(index, sender[index].ToDTO());
                }
            });
        }

        public WordDTO? Find(Guid id)
        {
            return _wordRepository.Find(id)?.ToDTO();
        }

        public async Task<WordDTO> CreateAsync(WordDTO dto)
        {
            dto.Id = Guid.NewGuid(); // Ensure a new ID is generated for the new word
            var entity = dto.ToEntity();

            using (var trans = await _wordRepository.BeginWriteAsync())
            {
                _wordRepository.Create(entity);

                await trans.CommitAsync();
            }

            return entity.ToDTO(); // Return the created entity as DTO
        }

        public async Task UpdateAsync(WordDTO dto)
        {
            var entity = _wordRepository.Find(dto.Id);

            if (entity == null)
            {
                throw new Exception($"Word with ID {dto.Id} not found.");
            }

            using (var trans = await _wordRepository.BeginWriteAsync())
            {
                entity.Text = dto.Text;
                entity.Translation = dto.Translation;
                entity.Definition = dto.Definition;

                await trans.CommitAsync();
            }   
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = _wordRepository.Find(id);

            if (entity == null)
            {
                throw new Exception($"Word with ID {id} not found.");
            }

            using (var trans = await _wordRepository.BeginWriteAsync())
            {
                _wordRepository.Delete(entity);

                await trans.CommitAsync();
            }
        }

        public void Dispose()
        {
            _realmSubscription?.Dispose();
            WordsSource.Dispose();
            FilterSubject.Dispose();
        }
    }
}
