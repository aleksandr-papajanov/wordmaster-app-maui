using Realms;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using WordMaster.Data.DTOs;
using WordMaster.Data.DTOs.Mappers;
using WordMaster.Data.Infrastructure;
using WordMaster.Data.Infrastructure.Interfaces;
using WordMaster.Data.Models;

namespace WordMaster.Data.Services
{
    public class WordService : IWordService
    {
        private readonly IRepository<Word> _wordRepository;
        private readonly BehaviorSubject<IList<WordDTO>> _words = new([]);
        private IRealmCollection<Word> _realmWords;

        public IObservable<IList<WordDTO>> Words => _words.AsObservable();

        public WordService(IRepository<Word> wordRepository)
        {
            _wordRepository = wordRepository;

            _realmWords = _wordRepository.All.AsRealmCollection();

            // publish of initial data
            _words.OnNext(_realmWords.Select(w => w.ToDTO()).ToList());

            // subscribe to changes in the Realm collection
            _realmWords.SubscribeForNotifications((sender, changes) =>
            {
                _words.OnNext(_realmWords.Select(w => w.ToDTO()).ToList());
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
    }
}
