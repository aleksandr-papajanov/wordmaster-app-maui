using WordMaster.Data.Models;
using WordMaster.Data.Services;
using WordMasterApp.EntityViewModels.Actions;

namespace WordMasterApp.EntityViewModels.DIFactories
{
    public interface IWordUsageEntityViewModelFactory
    {
        WordUsageEntityViewModel Create(WordUsage entity);
        WordUsageEntityViewModel Create(WordEntityViewModel parent);
    }

    public class WordUsageEntityViewModelFactory : IWordUsageEntityViewModelFactory
    {
        private readonly IWordService _wordService;
        private readonly Lazy<IWordEntityViewModelFactory> _wordFactory;

        public WordUsageEntityViewModelFactory(
            IWordService wordService,
            Lazy<IWordEntityViewModelFactory> wordFactory)
        {
            _wordService = wordService;
            _wordFactory = wordFactory;
        }

        private WordUsageEntityViewModelActions CreateActions(WordUsageEntityViewModel viewModel, WordUsage entity)
        {
             return new WordUsageEntityViewModelActions
             {
                GetParentWord = () => _wordFactory.Value.Create(entity.ParentWord),
                CreateAsync = async () => await _wordService.CreateUsageAsync(entity),
                DeleteAsync = async () => await _wordService.DeleteUsageAsync(entity),
                UpdateAsync = async updater => await _wordService.UpdateUsageAsync(entity, updater)
             };
        }

        public WordUsageEntityViewModel Create(WordUsage entity)
        {
            if (!entity.IsManaged)
            {
                throw new Exception("Can not create wrapper of detouched entity");
            }
            
            var viewModel = new WordUsageEntityViewModel(entity);
            viewModel.Actions = CreateActions(viewModel, entity);

            return viewModel;
        }

        public WordUsageEntityViewModel Create(WordEntityViewModel parent)
        {
            if (!parent.IsManaged)
            {
                throw new Exception("Can not create wrapper of detouched entity");
            }

            var entity = new WordUsage
            {
                Id = Guid.NewGuid(),
                ParentWord = parent.Entity
            };

            var viewModel = new WordUsageEntityViewModel(entity);
            viewModel.Actions = CreateActions(viewModel, entity);

            return viewModel;
        }
    }
}
