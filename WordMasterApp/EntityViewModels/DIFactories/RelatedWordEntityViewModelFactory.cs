using WordMaster.Data.Models;
using WordMaster.Data.Services.Interfaces;
using WordMasterApp.EntityViewModels.Actions;

namespace WordMasterApp.EntityViewModels.DIFactories
{
    public interface IRelatedWordEntityViewModelFactory
    {
        RelatedWordEntityViewModel Create(RelatedWord entity);
        RelatedWordEntityViewModel Create(WordEntityViewModel parent);
    }

    public class RelatedWordEntityViewModelFactory : IRelatedWordEntityViewModelFactory
    {
        private readonly IRelatedWordService _relatedWordService;
        private readonly IWordService _wordService;
        private readonly Lazy<IWordEntityViewModelFactory> _wordFactory;

        public RelatedWordEntityViewModelFactory(
            IRelatedWordService relatedWordService,
            IWordService wordService,
            Lazy<IWordEntityViewModelFactory> wordFactory)
        {
            _relatedWordService = relatedWordService;
            _wordService = wordService;
            _wordFactory = wordFactory;
        }

        private RelatedWordEntityViewModelActions CreateActions(RelatedWordEntityViewModel viewModel, RelatedWord entity)
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

                   await _relatedWordService.CreateAsync(entity);
               },
               async () =>
               {
                   if (!entity.IsManaged)
                   {
                       throw new Exception("Can not delete detouched entity");
                   }

                   await _relatedWordService.DeleteAsync(entity);
               },
               async updater =>
               {
                   if (!entity.IsManaged)
                   {
                       throw new Exception("Can not update detouched entity");
                   }

                   await _relatedWordService.UpdateAsync(entity, updater);
               }
           );
        }

        public RelatedWordEntityViewModel Create(RelatedWord entity)
        {
            if (!entity.IsManaged)
            {
                throw new Exception("Can not create wrapper of detouched entity");
            }

            var viewModel = new RelatedWordEntityViewModel(entity);
            viewModel.Actions = CreateActions(viewModel, entity);

            return viewModel;
        }

        public RelatedWordEntityViewModel Create(WordEntityViewModel parent)
        {
            if (!parent.IsManaged)
            {
                throw new Exception("Can not create wrapper of detouched entity");
            }

            var entity = new RelatedWord
            {
                WordId = parent.Id
            };

            var viewModel = new RelatedWordEntityViewModel(entity);
            viewModel.Actions = CreateActions(viewModel, entity);

            return viewModel;
        }
    }
}
