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
                SchemaVersion = 11,
                MigrationCallback = (_, _) => { },
                IsReadOnly = false, // Opens for change or creates a writable file if it doesn't exist
                Schema = new[]
                {
                    typeof(AppSettings),
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

            //var settings = _realm.Find<AppSettings>("global");

            //if (settings == null)
            //{
                
            //}
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
        #endregion
    }
}
