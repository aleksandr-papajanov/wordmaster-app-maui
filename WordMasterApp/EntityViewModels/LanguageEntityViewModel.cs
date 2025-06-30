using WordMaster.Data.Models;

namespace WordMasterApp.EntityViewModels
{
    public class LanguageEntityViewModel : EntityViewModelBase<Language>
    {
        public string Name { get; set; } = string.Empty;

        public string Code => _entity.Code;

        public LanguageEntityViewModel(Language entity) : base(entity)
        {
            if (!entity.IsManaged)
            {
                throw new Exception("Can not create wrapper of detouched entity");
            }
        }

        protected override Dictionary<string, UpdatePropertyMap> GetUpdateMap() => new()
        {
            {
                nameof(Language.Name),
                new (
                    () => Name = _entity.Name,
                    () => _entity.Name = Name)
            }
        };
    }
}