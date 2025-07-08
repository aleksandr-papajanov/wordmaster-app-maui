using Realms;

namespace WordMasterApp.EntityViewModels.Actions
{
    public class WordUsageEntityViewModelActions : EntityViewModelActionsBase
    {
        public required Func<WordEntityViewModel?> GetParentWord { get; set; }
        public required Func<Task> CreateAsync { get; set; }
        public required Func<Task> DeleteAsync { get; set; }
        public required Func<Action<Transaction>, Task> UpdateAsync { get; set; }
    }
}
