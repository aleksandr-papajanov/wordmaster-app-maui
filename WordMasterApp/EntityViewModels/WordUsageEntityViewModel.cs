using WordMaster.Data.Models;
using WordMasterApp.EntityViewModels.Actions;

namespace WordMasterApp.EntityViewModels
{
    public class WordUsageEntityViewModel : EntityViewModelBase<WordUsage>
    {
        private WordUsageEntityViewModelActions _actions;
        public WordUsageEntityViewModelActions Actions
        {
            set { _actions = value; }
        }

        // UI read-only properties
        public string Text { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;

        // Read-only properties
        public Guid Id => _entity.Id;
        public DateTimeOffset CreatedAt => _entity.CreatedAt;
        public Guid WordId => _entity.WordId;

        //Navigation properties
        public WordEntityViewModel? Word => _actions.GetParentWord();


        public WordUsageEntityViewModel(WordUsage entity) : base(entity) { }

        
        protected override Dictionary<string, UpdatePropertyMap> GetUpdateMap() => new()
        {
            {
                nameof(WordUsage.Text),
                new (
                    () => Text = _entity.Text,
                    () => _entity.Text = Text)
            },
            {
                nameof(WordUsage.Translation),
                new (
                    () => Translation = _entity.Translation,
                    () => _entity.Translation = Translation)
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

        public async Task DeleteAsync()
        {
            await _actions.DeleteAsync();
        }
    }
}


