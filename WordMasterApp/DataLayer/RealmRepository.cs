using Realms;

namespace WordMasterApp.DataLayer
{
    public class RealmRepository<T> : IRepository<T>
        where T : IRealmObject
    {
        private readonly RealmDataContext _context;

        public RealmRepository(RealmDataContext context)
        {
            _context = context;
        }

        public IQueryable<T> All => _context.Instance.All<T>();

        public T? Find(Guid id)
        {
            return _context.Instance.Find<T>(id);
        }
        
        public T? Find(string id)
        {
            return _context.Instance.Find<T>(id);
        }
        
        public async Task CreateAsync(T entity)
        {
            using (var transaction = await _context.Instance.BeginWriteAsync())
            {
                _context.Instance.Add(entity);
                transaction.Commit();
            }
        }

        public async Task UpdateAsync(T entity, Action<T, Transaction> action)
        {
            using (var transaction = await _context.Instance.BeginWriteAsync())
            {
                action(entity, transaction);

                _context.Instance.Add(entity, true);
                transaction.Commit();
            }
        }

        public async Task DeleteAsync(T entity)
        {
            using (var transaction = await _context.Instance.BeginWriteAsync())
            {
                _context.Instance.Remove(entity);
                transaction.Commit();
            }
        }
    }
}
