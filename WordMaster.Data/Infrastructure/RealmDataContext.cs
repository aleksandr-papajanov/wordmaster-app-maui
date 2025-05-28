using Realms;
using Realms.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMaster.Data.Exceptions;
using WordMaster.Data.Infrastructure.Interfaces;
using WordMaster.Data.Models;

namespace WordMaster.Data.Infrastructure
{
    internal class RealmDataContext : IDataContext
    {
        private readonly Realm _realm;
        private readonly string _dbPath;

        public Realm Instance => _realm;

        public RealmDataContext()
        {
            _dbPath = Path.Combine(AppContext.BaseDirectory, "wordmaster.realm");

            var config = new RealmConfiguration(_dbPath)
            {
                SchemaVersion = 3,
                MigrationCallback = (_, _) => { },
                IsReadOnly = false, // Opens for change or creates a writable file if it doesn't exist
                Schema = new[]
                {
                    typeof(Language),
                    typeof(Deck),
                    typeof(Word)
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

        public T Create<T>(T entity) where T : IRealmObject
        {
            return _realm.Write(() => _realm.Add(entity, false));
        }

        public T Update<T>(T entity) where T : IRealmObject
        {
            return _realm.Write(() => _realm.Add(entity, true));
        }

        public void Delete(IRealmObject entity)
        {
            _realm.Write(() => _realm.Remove(entity));
        }
        #endregion
    }
}
