using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMaster.Data.Infrastructure.Interfaces
{
    internal interface IDataContext
    {
        public IQueryable<T> All<T>() where T : IRealmObject;
        public T? Find<T>(Guid id) where T : IRealmObject;
        public T Create<T>(T entity) where T : IRealmObject;
        public T Update<T>(T entity) where T : IRealmObject;
        public void Delete(IRealmObject entity);
    }
}
