using Microsoft.Extensions.Configuration;
using Realms;
using WordMaster.Common;
using WordMaster.Data.Models;

namespace WordMaster.Data
{
    public class RealmDataContext
    {
        private readonly string _fileName;
        private readonly string _path;
        private readonly RealmConfiguration _realmConfiguration;

        private Realm? _realm;
        public Realm Instance => _realm ?? throw new InvalidOperationException("Realm is not opened yet. Call Open() first.");

        public RealmDataContext(IConfiguration config)
        {
            _fileName = config["AppSettings:Realm:FileName"] ?? throw new Exception("Realm file name is not configured in appsettings.json");
            _path = Path.Combine(AppContext.BaseDirectory, _fileName);

            _realmConfiguration = new RealmConfiguration(_path)
            {
                SchemaVersion = 1,
                MigrationCallback = (_, _) => { },
                IsReadOnly = false,
                Schema = new[]
                {
                    typeof(AppSettings),
                    typeof(Language),
                    typeof(Deck),
                    typeof(Word),
                    typeof(WordDeck),
                    typeof(WordDetails),
                    typeof(WordRelation),
                    typeof(WordUsage),
                }
            };
        }

        public void Open()
        {
            //EnsureInitialized();

            _realm = Realm.GetInstance(_realmConfiguration);

            //Seed();
        }

        private void Seed()
        {
            var lang1 = new Language
            {
                Name = "Swedish",
                Code = "sw"
            };

            var lang2 = new Language
            {
                Name = "Russian",
                Code = "ru"
            };

            var deck = new Deck
            {
                Id = Guid.NewGuid(),
                Name = "Default Deck",
                SourceLanguage = lang1,
                TargetLanguage = lang2
            };

            _realm.Write(() =>
            {
                _realm.Add(lang1);
                _realm.Add(lang2);
                _realm.Add(deck);
            });

            _realm.Write(() =>
            {
                var wordDetails = new WordDetails
                {
                    Id = Guid.NewGuid(),
                    Pronunciation = "həˈloʊ",
                    PartOfSpeechLabel = "Interjection",
                    PartOfSpeechType = (int)PartOfSpeech.Interjection
                };

                _realm.Add(wordDetails);

                var word = new Word
                {
                    Id = Guid.NewGuid(),
                    Text = "hello",
                    Definition = "A greeting or expression of goodwill.",
                    Translation = "привет",
                    UsageFrequency = (int)WordUsageFrequenсy.Common,
                    Details = wordDetails,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };

                _realm.Add(word);

                var wordDeck = new WordDeck
                {
                    Deck = deck,
                    Word = word
                };

                _realm.Add(wordDeck);

                var wordUsage1 = new WordUsage
                {
                    Id = Guid.NewGuid(),
                    ParentWord = word,
                    Text = "Hello, how are you?",
                    Translation = "Привет",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };

                var wordUsage2 = new WordUsage
                {
                    Id = Guid.NewGuid(),
                    ParentWord = word,
                    Text = "Hello, my friend!",
                    Translation = "Привет, мой друг!",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };

                _realm.Add(wordUsage1);
                _realm.Add(wordUsage2);

                var relatedWord1 = new Word
                {
                    Id = Guid.NewGuid(),
                    Text = "hi",
                    Definition = "A casual greeting.",
                    Translation = "привет",
                    UsageFrequency = (int)WordUsageFrequenсy.Common,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };

                var relatedWord2 = new Word
                {
                    Id = Guid.NewGuid(),
                    Text = "bye",
                    Definition = "A casual farewell.",
                    Translation = "пока",
                    UsageFrequency = (int)WordUsageFrequenсy.Common,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };

                _realm.Add(relatedWord1);
                _realm.Add(relatedWord2);

                var relation1 = new WordRelation
                {
                    Id = Guid.NewGuid(),
                    ParentWord = word,
                    RelatedWord = relatedWord1,
                    RelationType = (int)WordRelationType.Synonym,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };

                var relation2 = new WordRelation
                {
                    Id = Guid.NewGuid(),
                    ParentWord = word,
                    RelatedWord = relatedWord2,
                    RelationType = (int)WordRelationType.Antonym,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };

                _realm.Add(relation1);
                _realm.Add(relation2);
            });

            _realm.Write(() =>
            {
                var wordDetails = new WordDetails
                {
                    Id = Guid.NewGuid(),
                    Pronunciation = "wɜrld",
                    PartOfSpeechLabel = "Noun",
                    PartOfSpeechType = (int)PartOfSpeech.Noun
                };

                _realm.Add(wordDetails);

                var word = new Word
                {
                    Id = Guid.NewGuid(),
                    Text = "world",
                    Definition = "A term used to refer to the earth or the realm of human existence.",
                    Translation = "мир",
                    UsageFrequency = (int)WordUsageFrequenсy.Common,
                    Details = wordDetails,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };

                _realm.Add(word);

                var wordDeck = new WordDeck
                {
                    Deck = deck,
                    Word = word
                };

                _realm.Add(wordDeck);

                var wordUsage1 = new WordUsage
                {
                    Id = Guid.NewGuid(),
                    ParentWord = word,
                    Text = "Hello, world!",
                    Translation = "Привет, мир!",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };

                var wordUsage2 = new WordUsage
                {
                    Id = Guid.NewGuid(),
                    ParentWord = word,
                    Text = "World peace is important.",
                    Translation = "Мир во всем мире важен.",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };

                _realm.Add(wordUsage1);
                _realm.Add(wordUsage2);

                var relatedWord1 = new Word
                {
                    Id = Guid.NewGuid(),
                    Text = "earth",
                    Definition = "The planet we live on, the third from the sun.",
                    Translation = "земля",
                    UsageFrequency = (int)WordUsageFrequenсy.Common,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };

                var relatedWord2 = new Word
                {
                    Id = Guid.NewGuid(),
                    Text = "void",
                    Definition = "A state of emptiness or nothingness.",
                    Translation = "пустота",
                    UsageFrequency = (int)WordUsageFrequenсy.Common,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };

                _realm.Add(relatedWord1);
                _realm.Add(relatedWord2);

                var relation1 = new WordRelation
                {
                    Id = Guid.NewGuid(),
                    ParentWord = word,
                    RelatedWord = relatedWord1,
                    RelationType = (int)WordRelationType.Synonym,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };

                var relation2 = new WordRelation
                {
                    Id = Guid.NewGuid(),
                    ParentWord = word,
                    RelatedWord = relatedWord2,
                    RelationType = (int)WordRelationType.Antonym,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };

                _realm.Add(relation1);
                _realm.Add(relation2);
            });
        }

        private void EnsureInitialized()
        {
            if (!File.Exists(_path))
            {
                CopySeedDatabase();
                return;
            }

            using var tempRealm = Realm.GetInstance(_realmConfiguration);
            var settings = tempRealm.Find<AppSettings>("global");

            if (settings == null || !settings.IsInitialized)
            {
                tempRealm.Dispose();
                File.Delete(_path);
                CopySeedDatabase();
            }
        }

        private void CopySeedDatabase()
        {
            var assembly = typeof(RealmDataContext).Assembly;
            using var resourceStream = assembly.GetManifestResourceStream("WordMaster.Data.SeedDatabase.realm");

            if (resourceStream == null)
            {
                throw new FileNotFoundException("Seed database resource not found.");
            }

            using var fileStream = File.Create(_path);
            resourceStream.CopyTo(fileStream);
        }

        public IQueryable<T> All<T>() where T : IRealmObject
        {
            return Instance.All<T>();
        }

        public T? Find<T>(Guid id) where T : IRealmObject
        {
            return Instance.Find<T>(id);
        }
        
        public T? Find<T>(string id) where T : IRealmObject
        {
            return Instance.Find<T>(id);
        }

        public void Create<T>(T entity) where T : IRealmObject
        {
            Instance.Add(entity, false);
        }

        public void Update<T>(T entity) where T : IRealmObject
        {
            Instance.Add(entity, true);
        }

        public void Delete(IRealmObject entity)
        {
            Instance.Remove(entity);
        }
        public async Task<Transaction> BeginWriteAsync(CancellationToken cancellationToken = default)
        {
            return await Instance.BeginWriteAsync(cancellationToken);
        }
    }
}
