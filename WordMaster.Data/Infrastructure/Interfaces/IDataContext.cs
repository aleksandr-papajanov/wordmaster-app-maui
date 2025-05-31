using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMaster.Data.Infrastructure.Interfaces
{
    public interface IDataContext
    {
        public IQueryable<T> All<T>() where T : IRealmObject;
        public T? Find<T>(Guid id) where T : IRealmObject;
        public void Create<T>(T entity) where T : IRealmObject;
        public void Update<T>(T entity) where T : IRealmObject;
        public void Delete(IRealmObject entity);
        Task<Transaction> BeginWriteAsync(CancellationToken cancellationToken);
    }
}
