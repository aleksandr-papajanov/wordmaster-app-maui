using ReactiveUI;
using Realms;

namespace WordMasterApp.EntityWrappers
{
    public abstract class EntityWrapperBase<T> : ReactiveObject
        where T : RealmObject, new()
    {
        protected readonly T _entity;

        public bool IsManaged => _entity.IsManaged;


        protected EntityWrapperBase(T entity)
        {
            _entity = entity;
        }
    }
}
