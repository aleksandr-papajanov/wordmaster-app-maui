using Realms;

namespace WordMasterApp.EntityViewModels.Actions
{
    public readonly struct RelatedWordEntityViewModelActions
    {
        public Func<WordEntityViewModel?> GetParentWord { get; }
        public Func<Task> CreateAsync { get; }
        public Func<Task> DeleteAsync { get; }
        public Func<Action<Transaction>, Task> UpdateAsync { get; }

        public RelatedWordEntityViewModelActions(
            Func<WordEntityViewModel?> getParentWord,
            Func<Task> createAsync,
            Func<Task> deleteAsync,
            Func<Action<Transaction>, Task> updateAsync)
        {
            GetParentWord = getParentWord;
            CreateAsync = createAsync;
            DeleteAsync = deleteAsync;
            UpdateAsync = updateAsync;
        }
    }
}
