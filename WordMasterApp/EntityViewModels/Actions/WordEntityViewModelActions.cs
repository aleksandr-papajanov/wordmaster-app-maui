using DynamicData;
using Realms;
using WordMaster.Common;
using WordMaster.Generation.DTOs;
using WordMaster.Generation.Interfaces;

namespace WordMasterApp.EntityViewModels.Actions
{
    public readonly struct WordEntityViewModelActions
    {
        public Func<DeckEntityViewModel> GetParentDeck { get; }
        public Func<string, IObservable<IChangeSet<WordUsageEntityViewModel>>> GetUsagesStream { get; }
        public Func<WordRelationType, IObservable<IChangeSet<RelatedWordEntityViewModel>>> GetRelatedWordsStream { get; }
        public Func<Task> CreateAsync { get; }
        public Func<Task> DeleteAsync { get; }
        public Func<Action<Transaction>, Task> UpdateAsync { get; }
        public Func<WordCompletionRequest, IGenerationSession> CompleteWordFull { get; }

        public WordEntityViewModelActions(
            Func<DeckEntityViewModel> getParentDeck,
            Func<string, IObservable<IChangeSet<WordUsageEntityViewModel>>> getUsagesStream,
            Func<WordRelationType, IObservable<IChangeSet<RelatedWordEntityViewModel>>> getRelatedWordsStream,
            Func<Task> createAsync,
            Func<Task> deleteAsync,
            Func<Action<Transaction>, Task> updateAsync,
            Func<WordCompletionRequest, IGenerationSession> completeWordFull)
        {
            GetParentDeck = getParentDeck;
            GetUsagesStream = getUsagesStream;
            GetRelatedWordsStream = getRelatedWordsStream;
            CreateAsync = createAsync;
            DeleteAsync = deleteAsync;
            UpdateAsync = updateAsync;
            CompleteWordFull = completeWordFull;
        }
    }
}
