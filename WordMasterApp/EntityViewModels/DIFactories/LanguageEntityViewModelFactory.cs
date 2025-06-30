using WordMaster.Data.Models;

namespace WordMasterApp.EntityViewModels.DIFactories
{
    public interface ILanguageEntityViewModelFactory
    {
        LanguageEntityViewModel Create(Language entity);
    }

    public class LanguageEntityViewModelFactory : ILanguageEntityViewModelFactory
    {
        public LanguageEntityViewModel Create(Language entity)
        {
            return new LanguageEntityViewModel(entity);
        }
    }
}
