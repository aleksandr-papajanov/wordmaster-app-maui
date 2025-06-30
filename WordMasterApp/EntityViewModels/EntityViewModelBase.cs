using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using Realms;

namespace WordMasterApp.EntityViewModels
{
    public abstract class EntityViewModelBase<T> : ReactiveObject, IValidatableViewModel
        where T : RealmObject, new()
    {
        protected record UpdatePropertyMap(Action ToWrapper, Action ToEntity);

        protected readonly T _entity;
        protected readonly Dictionary<string, UpdatePropertyMap> _updateMap;

        public bool IsManaged => _entity.IsManaged;

        // Implementing IValidatableViewModel
        public IValidationContext ValidationContext { get; private set; } = null!;

        private ObservableAsPropertyHelper<bool>? _isValid;
        public bool IsValid => _isValid?.Value ?? true;

        public IObservable<bool>? IsValidChanged { get; private set; } 


        protected EntityViewModelBase(T entity)
        {
            _entity = entity;
            _updateMap = GetUpdateMap();

            _entity.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName != null && _updateMap.TryGetValue(e.PropertyName, out var actions))
                {
                    actions.ToEntity.Invoke();
                }
            };

            UpdateWrapper();
        }

        // Abstract method to define the mapping between entity properties and wrapper properties
        protected abstract Dictionary<string, UpdatePropertyMap> GetUpdateMap();

        protected virtual void SetupValidation()
        {
            ValidationContext = new ValidationContext();

            ValidationContext
                .WhenAnyValue(x => x.IsValid)
                .ToProperty(this, x => x.IsValid, out _isValid);

            IsValidChanged = ValidationContext.WhenAnyValue(x => x.IsValid); 
        }

        protected virtual void UpdateWrapper()
        {
            foreach (var map in _updateMap)
            {
                map.Value.ToWrapper.Invoke();
            }
        }
        
        public void UpdateEntity()
        {
            foreach (var map in _updateMap)
            {
                map.Value.ToEntity.Invoke();
            }
        }
    }
}
