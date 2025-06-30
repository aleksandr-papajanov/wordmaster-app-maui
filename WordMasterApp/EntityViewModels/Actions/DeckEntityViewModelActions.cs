using DynamicData;
using Realms;

namespace WordMasterApp.EntityViewModels.Actions
{
    public readonly struct DeckEntityViewModelActions
    {
        public Func<LanguageEntityViewModel> GetSourceLanguage { get; }
        public Func<LanguageEntityViewModel> GetTargetLanguage { get; }
        public Func<string, IObservable<IChangeSet<WordEntityViewModel>>> GetWordsStream { get; }
        public Func<Task> CreateAsync { get; }
        public Func<Task> DeleteAsync { get; }
        public Func<Action<Transaction>, Task> UpdateAsync { get; }

        public DeckEntityViewModelActions(
            Func<LanguageEntityViewModel> getSourceLanguage,
            Func<LanguageEntityViewModel> getTargetLanguage,
            Func<string, IObservable<IChangeSet<WordEntityViewModel>>> getWordsStream,
            Func<Task> createAsync,
            Func<Task> deleteAsync,
            Func<Action<Transaction>, Task> updateAsync)
        {
            GetSourceLanguage = getSourceLanguage;
            GetTargetLanguage = getTargetLanguage;
            GetWordsStream = getWordsStream;
            CreateAsync = createAsync;
            DeleteAsync = deleteAsync;
            UpdateAsync = updateAsync;
        }
    }
}
