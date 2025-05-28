using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMaster.Data.Infrastructure.Interfaces;

namespace WordMaster.Data.Infrastructure
{
    internal class RealmRepository<T> : IRepository<T>
        where T : IRealmObject
    {
        private readonly IDataContext _context;

        public RealmRepository(IDataContext context)
        {
            _context = context;
        }

        public IQueryable<T> All => _context.All<T>();

        public T? Find(Guid id)
        {
            return _context.Find<T>(id);
        }
        
        public void Create(T entity)
        {
            _context.Create(entity);
        }

        public void Update(T entity)
        {
            _context.Update(entity);
        }

        public void Delete(T entity)
        {
            _context.Delete(entity);
        }
    }
}
