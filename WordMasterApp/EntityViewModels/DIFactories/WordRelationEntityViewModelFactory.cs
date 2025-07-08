using WordMaster.Data.Models;
using WordMaster.Data.Services;
using WordMasterApp.EntityViewModels.Actions;

namespace WordMasterApp.EntityViewModels.DIFactories
{
    public interface IWordRelationEntityViewModelFactory
    {
        WordRelationEntityViewModel Create(WordRelation entity);
        WordRelationEntityViewModel Create(WordEntityViewModel parent, WordEntityViewModel related);
    }

    public class WordRelationEntityViewModelFactory : IWordRelationEntityViewModelFactory
    {
        private readonly IWordService _wordService;
        private readonly Lazy<IWordEntityViewModelFactory> _wordFactory;

        public WordRelationEntityViewModelFactory(
            IWordService wordService,
            Lazy<IWordEntityViewModelFactory> wordFactory)
        {
            _wordService = wordService;
            _wordFactory = wordFactory;
        }

        private RelatedWordEntityViewModelActions CreateActions(WordRelationEntityViewModel viewModel, WordRelation entity)
        {
            return new RelatedWordEntityViewModelActions
            {
                GetParentWord = () => _wordFactory.Value.Create(entity.ParentWord),
                GetRelatedWord = () => _wordFactory.Value.Create(entity.RelatedWord),
                CreateAsync = async () =>
                {
                    throw new NotImplementedException();
                },
                DeleteAsync = async () =>
                {
                    throw new NotImplementedException();
                },
                UpdateAsync = async updater =>
                {
                    throw new NotImplementedException();
                }
            };
        }

        public WordRelationEntityViewModel Create(WordRelation entity)
        {
            if (!entity.IsManaged)
            {
                throw new Exception("Can not create wrapper of detouched entity");
            }

            var viewModel = new WordRelationEntityViewModel(entity);
            viewModel.Actions = CreateActions(viewModel, entity);

            return viewModel;
        }

        public WordRelationEntityViewModel Create(WordEntityViewModel parent, WordEntityViewModel related)
        {
            if (!parent.IsManaged || !related.IsManaged)
            {
                throw new Exception("Can not create wrapper of detouched entity");
            }

            var entity = new WordRelation
            {
                Id = Guid.NewGuid(),
                ParentWord = parent.Entity,
                RelatedWord = related.Entity
            };

            var viewModel = new WordRelationEntityViewModel(entity);
            viewModel.Actions = CreateActions(viewModel, entity);

            return viewModel;
        }
    }
}
