using ReactiveUI;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMaster.Data.Models;

namespace WordMasterApp.EntityWrappers
{
    public abstract class EntityWrapperBase<T> : ReactiveObject
        where T : RealmObject, new()
    {
        protected readonly T _entity;

        public T Entity => _entity;
        public bool IsManaged => _entity.IsManaged;

        protected EntityWrapperBase(T entity)
        {
            _entity = entity;
        }
    }
}
