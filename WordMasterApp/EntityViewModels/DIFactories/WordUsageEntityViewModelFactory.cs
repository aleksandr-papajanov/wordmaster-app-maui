using WordMaster.Data.Models;
using WordMaster.Data.Services.Interfaces;
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
        private readonly IWordUsageService _wordUsageService;
        private readonly IWordService _wordService;
        private readonly Lazy<IWordEntityViewModelFactory> _wordFactory;

        public WordUsageEntityViewModelFactory(
            IWordUsageService wordUsageService,
            IWordService wordService,
            Lazy<IWordEntityViewModelFactory> wordFactory)
        {
            
            _wordUsageService = wordUsageService;
            _wordService = wordService;
            _wordFactory = wordFactory;
        }

        private WordUsageEntityViewModelActions CreateActions(WordUsageEntityViewModel viewModel, WordUsage entity)
        {
             return new(
                () =>
                {
                    var word = _wordService.Find(viewModel.WordId);

                    if (word != null)
                    {
                        return _wordFactory.Value.Create(word);
                    }

                    throw new Exception("Parent word not found");
                },
                async () =>
                {
                    if (entity.IsManaged)
                    {
                        throw new Exception("Can not create entity that already exists");
                    }

                    await _wordUsageService.CreateAsync(entity);
                },
                async () =>
                {
                    if (!entity.IsManaged)
                    {
                        throw new Exception("Can not delete detouched entity");
                    }

                    await _wordUsageService.DeleteAsync(entity);
                },
                async updater =>
                {
                    if (!entity.IsManaged)
                    {
                        throw new Exception("Can not update detouched entity");
                    }

                    await _wordUsageService.UpdateAsync(entity, updater);
                }
            );
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
                WordId = parent.Id
            };

            var viewModel = new WordUsageEntityViewModel(entity);
            viewModel.Actions = CreateActions(viewModel, entity);

            return viewModel;
        }
    }
}
