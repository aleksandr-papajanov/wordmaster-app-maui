using DynamicData;
using Realms;
using WordMaster.Common;
using WordMaster.Generation.DTOs;
using WordMaster.Generation.Interfaces;

namespace WordMasterApp.EntityViewModels.Actions
{
    public class WordEntityViewModelActions : EntityViewModelActionsBase
    {
        public required Func<WordDetailsEntityViewModel?> GetDetails { get; set; }
        public required Func<DeckEntityViewModel> GetParentDeck { get; set; }
        public required Func<string, string, PartOfSpeech, Task> CreateDetails { get; set; }
        public required Func<string, WordRelationType, Task> CreateRelation { get; set; }
        public required Func<string, string, Task> CreateUsage { get; set; }
        public required Func<string, IObservable<IChangeSet<WordUsageEntityViewModel>>> GetUsagesStream { get; set; }
        public required Func<WordRelationType, IObservable<IChangeSet<WordRelationEntityViewModel>>> GetRelatedWordsStream { get; set; }
        public required Func<Task> CreateAsync { get; set; }
        public required Func<DeckEntityViewModel, Task> DeleteAsync { get; set; }
        public required Func<Action<Transaction>, Task> UpdateAsync { get; set; }
    }
}
