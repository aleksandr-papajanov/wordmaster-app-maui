using WordMaster.Data.Models;
using WordMaster.Data.Services;
using WordMasterApp.EntityViewModels.Actions;

namespace WordMasterApp.EntityViewModels.DIFactories
{
    public interface IWordDetailsEntityViewModelFactory
    {
        WordDetailsEntityViewModel Create(WordDetails entity);
        WordDetailsEntityViewModel Create(WordEntityViewModel parent);
    }

    public class WordDetailsEntityViewModelFactory : IWordDetailsEntityViewModelFactory
    {
        private readonly IWordService _wordService;
        private readonly Lazy<IWordEntityViewModelFactory> _wordFactory;

        public WordDetailsEntityViewModelFactory(
            IWordService wordService,
            Lazy<IWordEntityViewModelFactory> wordFactory)
        {
            _wordService = wordService;
            _wordFactory = wordFactory;
        }

        private WordDetailsEntityViewModelActions CreateActions(WordDetailsEntityViewModel viewModel, WordDetails entity, Word? parent = null)
        {
            return new WordDetailsEntityViewModelActions
            {
                
                GetParentWord = () =>
                {
                    var found = entity.Words.FirstOrDefault();

                    if (found != null)
                    {
                        return _wordFactory.Value.Create(found);
                    }

                    return null;
                },
                CreateAsync = () => _wordService.CreateDetailsAsync(parent, entity),
                DeleteAsync = () => _wordService.DeleteDetailsAsync(entity),
                UpdateAsync = updater => _wordService.UpdateDetailsAsync(entity, updater)
            };
        }

        public WordDetailsEntityViewModel Create(WordDetails entity)
        {
            if (!entity.IsManaged)
            {
                throw new Exception("Can not create wrapper of detouched entity");
            }

            var viewModel = new WordDetailsEntityViewModel(entity);
            viewModel.Actions = CreateActions(viewModel, entity);

            return viewModel;
        }

        public WordDetailsEntityViewModel Create(WordEntityViewModel parent)
        {
            if (!parent.IsManaged)
            {
                throw new Exception("Can not create wrapper of detouched entity");
            }

            var entity = new WordDetails
            {
                Id = Guid.NewGuid(),
                //Word = parent.Entity
            };

            var viewModel = new WordDetailsEntityViewModel(entity);
            viewModel.Actions = CreateActions(viewModel, entity, parent.Entity);

            return viewModel;
        }
    }
}
