using Realms;

namespace WordMaster.Data
{
    public interface IRepository<T>
        where T : IRealmObject
    {
        IQueryable<T> All { get; }

        T? Find(Guid id);
        T? Find(string id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity, Action<Transaction> action);
        Task DeleteAsync(T entity);
    }
}
