using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMaster.Data.Infrastructure
{
    public interface IRepository<T>
        where T : IRealmObject
    {
        IQueryable<T> All { get; }

        T? Find(Guid id);
        T? Find(string id);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<Transaction> BeginWriteAsync(CancellationToken cancellationToken = default);
    }
}
