using WordMaster.Common;
using WordMaster.Data.Models;
using WordMasterApp.EntityViewModels.Actions;

namespace WordMasterApp.EntityViewModels
{
    public class WordRelationEntityViewModel : EntityViewModelBase<WordRelation>
    {
        private RelatedWordEntityViewModelActions? _actions;
        public RelatedWordEntityViewModelActions Actions
        {
            get => _actions ?? throw new InvalidOperationException("Actions must be set before using this view model.");
            set { _actions = value; }
        }

        private WordEntityViewModel? _relatedWord;
        public WordEntityViewModel RelatedWord => _relatedWord ??= Actions.GetRelatedWord();

        // UI read-only properties
        public WordRelationType RelationType { get; set; }
        public bool IsCompleted { get; private set; }

        // Read-only properties
        public Guid ParentWordId => _entity.ParentWord.Id;
        

        public WordRelationEntityViewModel(WordRelation entity) : base(entity) { }


        protected override Dictionary<string, UpdatePropertyMap> GetUpdateMap() => new()
        {
            {
                nameof(WordRelation.RelationType),
                new (
                    () => RelationType = (WordRelationType)_entity.RelationType,
                    () => _entity.RelationType = (int)RelationType)
            },
            {
                nameof(WordRelation.IsCompleted),
                new (
                    () => IsCompleted = _entity.IsCompleted,
                    () => _entity.IsCompleted = IsCompleted)
            }
        };

        public async Task SaveAsync()
        {
            if (IsManaged)
            {
                await Actions.UpdateAsync((t) =>
                {
                    IsCompleted = true;
                    UpdateEntity();
                });
            }
            else
            {
                UpdateEntity();

                await Actions.CreateAsync();
            }
        }
    }
}