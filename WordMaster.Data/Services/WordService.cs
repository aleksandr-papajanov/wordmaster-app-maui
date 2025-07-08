using DynamicData;
using DynamicData.Binding;
using Realms;
using System.Reactive.Linq;
using WordMaster.Common;
using WordMaster.Data.Models;

namespace WordMaster.Data.Services
{
    public interface IWordService
    {
        IObservable<IChangeSet<Word>> GetWordsStream(Deck deck, string filter);
        IObservable<IChangeSet<WordRelation>> GetRelationsStream(Word word, WordRelationType relationType);
        IObservable<IChangeSet<WordUsage>> GetUsagesStream(Word word, string filter);

        Task CreateWordAsync(Word word, Deck deck);
        Task DeleteWordFromDeckAsync(Word word, Deck deck);
        Task UpdateWordAsync(Word word, Action<Transaction> updater);

        Task CreateDetailsAsync(Word word, string pronunciation, string partOfSpeechLabel, PartOfSpeech partOfSpeechType);
        Task CreateDetailsAsync(Word parent, WordDetails details);
        Task DeleteDetailsAsync(WordDetails details);
        Task UpdateDetailsAsync(WordDetails details, Action<Transaction> updater);

        Task CreateUsageAsync(WordUsage usage);
        Task CreateUsageAsync(Word parent, string sentence, string translation);
        Task DeleteUsageAsync(WordUsage usage);
        Task UpdateUsageAsync(WordUsage usage, Action<Transaction> updater);

        Task CreateRelationAsync(WordRelation relation);
        Task CreateRelationAsync(Word parent, string word, WordRelationType relationType);
        Task DeleteRelationAsync(WordRelation relation);
        Task UpdateRelationAsync(WordRelation relation, Action<Transaction> updater);
    }

    public class WordService : IWordService
    {
        private readonly IRepository<Word> _wordRepository;
        private readonly IRepository<WordDeck> _wordDeckRepository;
        private readonly IRepository<WordDetails> _wordDetailsRepository;
        private readonly IRepository<WordUsage> _wordUsageRepository;
        private readonly IRepository<WordRelation> _wordRelationRepository;

        public WordService(
            IRepository<Word> wordRepository,
            IRepository<WordDeck> wordDeckRepository,
            IRepository<WordDetails> wordDetailsRepository,
            IRepository<WordUsage> wordUsageRepository,
            IRepository<WordRelation> wordRelationRepository)
        {
            _wordRepository = wordRepository;
            _wordDeckRepository = wordDeckRepository;
            _wordDetailsRepository = wordDetailsRepository;
            _wordUsageRepository = wordUsageRepository;
            _wordRelationRepository = wordRelationRepository;
        }


        public IObservable<IChangeSet<Word>> GetWordsStream(Deck deck, string filter)
        {
            var query = _wordDeckRepository.All
                .Where(x => x.Deck == deck);

            return query
                .AsRealmCollection()
                .ToObservableChangeSet<IRealmCollection<WordDeck>, WordDeck>()
                .Transform(x => x.Word)
                .Filter(x => (x.Text ?? "").Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                             (x.Translation ?? "").Contains(filter, StringComparison.OrdinalIgnoreCase));
        }

        public IObservable<IChangeSet<WordRelation>> GetRelationsStream(Word word, WordRelationType relationType)
        {
            var query = _wordRelationRepository.All
               .Where(x => x.ParentWord == word)
               .Where(x => x.RelationType == (int)relationType);

            return query
                .AsRealmCollection()
                .ToObservableChangeSet<IRealmCollection<WordRelation>, WordRelation>();
        }

        public IObservable<IChangeSet<WordUsage>> GetUsagesStream(Word word, string filter)
        {
            var query = _wordUsageRepository.All
                .Where(x => x.ParentWord == word)
                .Where(x => x.Text.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                            x.Translation.Contains(filter, StringComparison.OrdinalIgnoreCase));

            return query
                .AsRealmCollection()
                .ToObservableChangeSet<IRealmCollection<WordUsage>, WordUsage>();
        }

        // Word management methods
        public async Task CreateWordAsync(Word word, Deck deck)
        {
            EnsureUnmanaged(word, nameof(word));
            EnsureManaged(deck, nameof(deck));

            // Create word-deck association
            var wordDeck = new WordDeck
            {
                Deck = deck,
                Word = word
            };

            // Update word properties
            word.Id = Guid.NewGuid();
            word.CreatedAt = DateTimeOffset.UtcNow;
            word.ModifiedAt = DateTimeOffset.UtcNow;

            // Add to database
            await _wordRepository.CreateAsync(word);
            await _wordDeckRepository.CreateAsync(wordDeck);
        }

        public async Task DeleteWordFromDeckAsync(Word word, Deck deck)
        {
            // Ensure word is added to the specified deck before deleting
            var wordDeck = FindWordDeck(word, deck) ?? throw new InvalidOperationException("Word is not associated with the specified deck.");
            await _wordDeckRepository.DeleteAsync(wordDeck);

            // Deletes recursively the word and all its relations and usages if not associated with any other deck
            await DeleteWordAsync(word);
        }

        private async Task DeleteWordAsync(Word word)
        {
            EnsureManaged(word, nameof(word));

            // If the word is still associated with other decks stop here
            if (IsAssociatedWithAnyDeck(word))
            {
                return;
            }

            // if the word is associated with details entry, delete it
            if (word.Details != null)
            {
                await DeleteDetailsAsync(word.Details);
            }

            // save all the relations firs
            var relations = GetAllRelations(word);
            var usages = GetAllUsages(word);

            // Delete the word itself
            await _wordRepository.DeleteAsync(word);

            // Delete all usages
            foreach (var usage in usages)
            {
                await DeleteUsageAsync(usage);
            }

            // Delete all relations
            foreach (var relation in relations)
            {
                await DeleteRelationAsync(relation);
            }
        }

        public async Task UpdateWordAsync(Word word, Action<Transaction> updater)
        {
            EnsureManaged(word, nameof(word));
            await _wordRepository.UpdateAsync(word, transaction =>
            {
                updater(transaction);
                word.ModifiedAt = DateTime.UtcNow;
            });
        }

        // Word details management methods
        public async Task CreateDetailsAsync(Word word, string pronunciation, string partOfSpeechLabel, PartOfSpeech partOfSpeechType)
        {
            EnsureManaged(word, nameof(Word));

            if (word.Details != null)
            {
                throw new InvalidOperationException("Word already has details associated with it.");
            }

            var details = new WordDetails
            {
                Id = Guid.NewGuid(),
                Pronunciation = pronunciation,
                PartOfSpeechLabel = partOfSpeechLabel,
                PartOfSpeechType = (int)partOfSpeechType,
            };

            await CreateDetailsAsync(word, details);
        }

        public async Task CreateDetailsAsync(Word parent, WordDetails details)
        {
            EnsureUnmanaged(details, nameof(WordDetails));
            await _wordDetailsRepository.CreateAsync(details);
            await _wordRepository.UpdateAsync(parent, transaction =>
            {
                parent.Details = details;
                parent.ModifiedAt = DateTimeOffset.UtcNow;
            });
        }

        public async Task DeleteDetailsAsync(WordDetails details)
        {
            EnsureManaged(details, nameof(details));
            await _wordDetailsRepository.DeleteAsync(details);
        }

        public async Task UpdateDetailsAsync(WordDetails details, Action<Transaction> updater)
        {
            EnsureManaged(details, nameof(details));
            await _wordDetailsRepository.UpdateAsync(details, transaction => updater(transaction));
        }


        // Word usage management methods
        public async Task CreateUsageAsync(WordUsage usage)
        {
            EnsureUnmanaged(usage, nameof(usage));
            EnsureManaged(usage.ParentWord, nameof(usage.ParentWord));

            usage.Id = Guid.NewGuid();
            usage.CreatedAt = DateTimeOffset.UtcNow;    
            usage.ModifiedAt = DateTimeOffset.UtcNow;
            await _wordUsageRepository.CreateAsync(usage);
        }

        public async Task CreateUsageAsync(Word parent, string sentence, string translation)
        {
            EnsureManaged(parent, nameof(parent));

            var usage = _wordUsageRepository.All
                .FirstOrDefault(x => x.Text == sentence);

            // Create a new word if it doesn't exist
            if (usage == null)
            {
                usage = new WordUsage
                {
                    Id = Guid.NewGuid(),
                    ParentWord = parent,
                    Text = sentence,
                    Translation = translation,
                };

                await CreateUsageAsync(usage);
            }
        }

        public async Task DeleteUsageAsync(WordUsage usage)
        {
            EnsureManaged(usage, nameof(usage));
            await _wordUsageRepository.DeleteAsync(usage);
        }

        public async Task UpdateUsageAsync(WordUsage usage, Action<Transaction> updater)
        {
            EnsureManaged(usage, nameof(usage));
            await _wordUsageRepository.UpdateAsync(usage, transaction =>
            {
                updater(transaction);
                usage.ModifiedAt = DateTime.UtcNow;
            });
        }

        // Word relation management methods
        public async Task CreateRelationAsync(WordRelation relation)
        {
            EnsureUnmanaged(relation, nameof(relation));
            EnsureManaged(relation.ParentWord, nameof(relation.ParentWord));
            EnsureManaged(relation.RelatedWord, nameof(relation.RelatedWord));

            relation.CreatedAt = DateTimeOffset.UtcNow;
            relation.ModifiedAt = DateTimeOffset.UtcNow;
            await _wordRelationRepository.CreateAsync(relation);
        }

        public async Task CreateRelationAsync(Word parent, string word, WordRelationType relationType)
        {
            EnsureManaged(parent, nameof(parent));

            WordRelation? relation = null;
            var related = _wordRepository.All
                .FirstOrDefault(x => x.Text == word);

            // Create a new word if it doesn't exist
            if (related == null)
            {
                related = new Word
                {
                    Id = Guid.NewGuid(),
                    Text = word,
                    CreatedAt = DateTimeOffset.UtcNow,
                    ModifiedAt = DateTimeOffset.UtcNow
                };

                await _wordRepository.CreateAsync(related);
            }
            else
            {
                // If the word already exists, check if the relation already exists
                relation = _wordRelationRepository.All
                    .FirstOrDefault(x => x.ParentWord == parent &&
                                         x.RelatedWord == related &&
                                         x.RelationType == (int)relationType);
            }

            // Otherwise, create a new relation
            if (relation == null)
            {
                relation = new WordRelation
                {
                    Id = Guid.NewGuid(),
                    ParentWord = parent,
                    RelatedWord = related,
                    RelationType = (int)relationType,
                };

                await CreateRelationAsync(relation);
            }
        }

        public async Task DeleteRelationAsync(WordRelation relation)
        {
            EnsureManaged(relation, nameof(relation));

            var relatedWord = relation.RelatedWord;

            // remove relation first
            await _wordRelationRepository.DeleteAsync(relation);

            // recursively delete to avoid orphaned relations
            await DeleteWordAsync(relatedWord);
        }

        public async Task UpdateRelationAsync(WordRelation relation, Action<Transaction> updater)
        {
            EnsureManaged(relation, nameof(relation));
            await _wordRelationRepository.UpdateAsync(relation, transaction =>
            {
                updater(transaction);
                relation.ModifiedAt = DateTime.UtcNow;
            });
        }


        // Provate helper methods
        private IEnumerable<WordRelation> GetAllRelations(Word word)
        {
            return _wordRelationRepository.All
                .Where(x => x.ParentWord == word)
                .ToList();
        }
        
        private IEnumerable<WordUsage> GetAllUsages(Word word)
        {
            return _wordUsageRepository.All
                .Where(x => x.ParentWord == word)
                .ToList();
        }

        private bool IsAssociatedWithAnyDeck(Word word)
        {
            return word.IsManaged && _wordDeckRepository.All
                .Any(x => x.Word == word);
        }

        private WordDeck? FindWordDeck(Word word, Deck deck)
        {
            var entry = _wordDeckRepository.All
                .FirstOrDefault(x => x.Deck == deck &&
                                     x.Word == word);

            return entry;
        }

        private void EnsureManaged(RealmObject entity, string argName = "unspecified")
        {
            if (!entity.IsManaged)
            {
                throw new InvalidOperationException($"Entity '{argName}' must be managed to perform this operation.");
            }
        }

        private void EnsureUnmanaged(RealmObject entity, string argName = "unspecified")
        {
            if (entity.IsManaged)
            {
                throw new InvalidOperationException($"Entity '{argName}' must be unmanaged to perform this operation.");
            }
        }
    }
}
