﻿using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMaster.Data.Infrastructure
{
    public class Repository<T> : IRepository<T>
        where T : IRealmObject
    {
        private readonly IDataContext _context;

        public Repository(IDataContext context)
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

        public async Task<Transaction> BeginWriteAsync(CancellationToken cancellationToken)
        {
            return await _context.BeginWriteAsync(cancellationToken);
        }
    }
}
