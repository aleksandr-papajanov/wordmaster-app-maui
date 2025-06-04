using Realms;
using Realms.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMaster.Data.Exceptions;
using WordMaster.Data.Models;

namespace WordMaster.Data.Infrastructure
{
    public class RealmDataContext : IDataContext
    {
        private readonly Realm _realm;
        private readonly string _dbPath;

        public Realm Instance => _realm;

        public RealmDataContext()
        {
            _dbPath = Path.Combine(AppContext.BaseDirectory, "wordmaster.realm");

            var config = new RealmConfiguration(_dbPath)
            {
                SchemaVersion = 9,
                MigrationCallback = (_, _) => { },
                IsReadOnly = false, // Opens for change or creates a writable file if it doesn't exist
                Schema = new[]
                {
                    typeof(Language),
                    typeof(Deck),
                    typeof(Word),
                    typeof(WordUsage)
                }
            };

            try
            {
                _realm = Realm.GetInstance(config);
            }
            catch (RealmFileAccessErrorException ex)
            {
                throw new DataLayerException(ex.Message);
            }

            SeedData();


            Console.WriteLine("WordMaster Data Layer");
        }

        private void SeedData()
        {
            if (!_realm.All<Language>().Any())
            {
                var languages = new List<Language>
                {
                    new Language { Code = "en", Name = "English" },
                    new Language { Code = "ru", Name = "Russian" },
                    new Language { Code = "sv", Name = "Swedish" },
                };

                _realm.Write(() =>
                {
                    foreach (var lang in languages)
                    {
                        _realm.Add(lang);
                    }
                });

                Console.WriteLine("Seeded predefined languages.");
            }

            if (!_realm.All<Word>().Any())
            {
                var words = new List<Word>
                {
                    new Word { Id = Guid.NewGuid(), Text = "Hello", Definition = "A greeting", Translation = "Привет" },
                    new Word { Id = Guid.NewGuid(), Text = "World", Definition = "The earth, together with all of its countries and peoples", Translation = "Мир" },
                    new Word { Id = Guid.NewGuid(), Text = "Hej", Definition = "Ett hälsningsord", Translation = "Hello" },
                    new Word { Id = Guid.NewGuid(), Text = "Världen", Definition = "Jorden, tillsammans med alla dess länder och folk", Translation = "World" }
                };

                _realm.Write(() =>
                {
                    foreach (var word in words)
                    {
                        _realm.Add(word);
                    }
                });

                Console.WriteLine("Seeded predefined words.");
            }
        }

        #region IDataContext Implementation
        public IQueryable<T> All<T>() where T : IRealmObject
        {
            return _realm.All<T>();
        }

        public T? Find<T>(Guid id) where T : IRealmObject
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
        #endregion
    }
}
