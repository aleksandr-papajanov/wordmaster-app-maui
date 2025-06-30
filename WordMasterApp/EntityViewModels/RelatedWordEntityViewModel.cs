using WordMaster.Common;
using WordMaster.Data.Models;
using WordMasterApp.EntityViewModels.Actions;

namespace WordMasterApp.EntityViewModels
{
    public class RelatedWordEntityViewModel : EntityViewModelBase<RelatedWord>
    {
        private RelatedWordEntityViewModelActions _actions;
        public RelatedWordEntityViewModelActions Actions
        {
            set { _actions = value; }
        }

        // UI read-only properties
        public string Text { get; set; } = string.Empty;
        public string? Translation { get; set; }
        public string? Definition { get; set; }
        public WordUsageFrequenсy? UsageFrequency { get; set; }
        public WordRelationType RelationType { get; set; }

        // Read-only properties
        public Guid Id => _entity.Id;
        public Guid WordId => _entity.WordId;


        public RelatedWordEntityViewModel(RelatedWord entity) : base(entity) { }


        protected override Dictionary<string, UpdatePropertyMap> GetUpdateMap() => new()
        {
            {
                nameof(RelatedWord.Text),
                new (
                    () => Text = _entity.Text,
                    () => _entity.Text = Text)
            },
            {
                nameof(RelatedWord.Translation),
                new (
                    () => Translation = _entity.Translation,
                    () => _entity.Translation = Translation)
            },
            {
                nameof(RelatedWord.Definition),
                new (
                    () => Definition = _entity.Definition,
                    () => _entity.Definition = Definition)
            },
            {
                nameof(RelatedWord.UsageFrequency),
                new (
                    () => UsageFrequency = (WordUsageFrequenсy?)_entity.UsageFrequency,
                    () => _entity.UsageFrequency = (int?)UsageFrequency)
            },
            {
                nameof(RelatedWord.RelationType),
                new (
                    () => RelationType = (WordRelationType)_entity.RelationType,
                    () => _entity.RelationType = (int)RelationType)
            }
        };

        public async Task SaveAsync()
        {
            if (IsManaged)
            {
                await _actions.UpdateAsync((t) =>
                {
                    UpdateEntity();
                });
            }
            else
            {
                UpdateEntity();

                await _actions.CreateAsync();
            }
        }
    }
}