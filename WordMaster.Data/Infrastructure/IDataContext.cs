using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMaster.Data.Infrastructure
{
    public interface IDataContext
    {
        IQueryable<T> All<T>() where T : IRealmObject;
        T? Find<T>(Guid id) where T : IRealmObject;
        T? Find<T>(string id) where T : IRealmObject;
        void Create<T>(T entity) where T : IRealmObject;
        void Update<T>(T entity) where T : IRealmObject;
        void Delete(IRealmObject entity);
        Task<Transaction> BeginWriteAsync(CancellationToken cancellationToken);
    }
}
