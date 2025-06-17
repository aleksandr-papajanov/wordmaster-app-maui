using Realms;

namespace WordMasterApp.DataLayer
{
    public interface IRepository<T>
        where T : IRealmObject
    {
        IQueryable<T> All { get; }

        T? Find(Guid id);
        T? Find(string id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity, Action<T, Transaction> action);
        Task DeleteAsync(T entity);
    }
}
