using Microsoft.Extensions.Configuration;
using Realms;
using WordMaster.Data.Models;

namespace WordMaster.Data
{
    public class RealmDataContext
    {
        private readonly string _fileName;
        private readonly string _path;
        private readonly RealmConfiguration _realmConfiguration;

        private Realm _realm;
        public Realm Instance => _realm ?? throw new InvalidOperationException("Realm is not opened yet. Call Open() first.");

        public RealmDataContext(IConfiguration config)
        {
            _fileName = config["AppSettings:Realm:FileName"] ?? throw new Exception("Realm file name is not configured in appsettings.json");
            _path = Path.Combine(AppContext.BaseDirectory, _fileName);

            _realmConfiguration = new RealmConfiguration(_path)
            {
                SchemaVersion = 21,
                MigrationCallback = (_, _) => { },
                IsReadOnly = false,
                Schema = new[]
                {
                    typeof(AppSettings),
                    typeof(Language),
                    typeof(Deck),
                    typeof(Word),
                    typeof(RelatedWord),
                    typeof(WordUsage),
                }
            };
        }

        public void Open()
        {
            EnsureInitialized();

            _realm = Realm.GetInstance(_realmConfiguration);

            //var words = _realm.All<Word>().ToList();

            //foreach(var word in words)
            //{
            //    var syns = word.Synonyms?.Split(",");

            //    if (syns != null && syns.Length > 0)
            //    {
            //        foreach (var syn in syns)
            //        {
            //            if (!string.IsNullOrWhiteSpace(syn))
            //            {
            //                var relatedWord = new RelatedWord
            //                {
            //                    Id = Guid.NewGuid(),
            //                    WordId = word.Id,
            //                    Text = syn.Trim(),
            //                    RelationType = (int)WordRelationType.Synonym
            //                };
            //                _realm.Write(() => { _realm.Add(relatedWord, false); });
  
            //            }
            //        }
            //    }

            //    var ants = word.Antonyms?.Split(",");

            //    if (ants != null && ants.Length > 0)
            //    {
            //        foreach (var ant in ants)
            //        {
            //            if (!string.IsNullOrWhiteSpace(ant))
            //            {
            //                var relatedWord = new RelatedWord
            //                {
            //                    Id = Guid.NewGuid(),
            //                    WordId = word.Id,
            //                    Text = ant.Trim(),
            //                    RelationType = (int)WordRelationType.Antonym
            //                };
            //                _realm.Write(() => { _realm.Add(relatedWord, false); });
            //            }
            //        }
            //    }
            //}
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
            using var resourceStream = assembly.GetManifestResourceStream("WordMasterApp.Resources.Raw.SeedDatabase.realm");

            if (resourceStream == null)
            {
                throw new FileNotFoundException("Seed database resource not found.");
            }

            using var fileStream = File.Create(_path);
            resourceStream.CopyTo(fileStream);
        }

        public IQueryable<T> All<T>() where T : IRealmObject
        {
            return _realm.All<T>();
        }

        public T? Find<T>(Guid id) where T : IRealmObject
        {
            return _realm.Find<T>(id);
        }
        
        public T? Find<T>(string id) where T : IRealmObject
        {
            return _realm.Find<T>(id);
        }

        public void Create<T>(T entity) where T : IRealmObject
        {
            _realm.Add(entity, false);
        }

        public void Update<T>(T entity) where T : IRealmObject
        {
            _realm.Add(entity, true);
        }

        public void Delete(IRealmObject entity)
        {
            _realm.Remove(entity);
        }
        public async Task<Transaction> BeginWriteAsync(CancellationToken cancellationToken = default)
        {
            return await _realm.BeginWriteAsync(cancellationToken);
        }
    }
}
