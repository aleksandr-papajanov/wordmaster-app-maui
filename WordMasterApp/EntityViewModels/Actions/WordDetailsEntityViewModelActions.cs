using DynamicData;
using Realms;
using WordMaster.Common;
using WordMaster.Generation.DTOs;
using WordMaster.Generation.Interfaces;

namespace WordMasterApp.EntityViewModels.Actions
{
    public class WordDetailsEntityViewModelActions : EntityViewModelActionsBase
    {
        public required Func<WordEntityViewModel?> GetParentWord { get; set; }
        public required Func<Task> CreateAsync { get; set; }
        public required Func<Task> DeleteAsync { get; set; }
        public required Func<Action<Transaction>, Task> UpdateAsync { get; set; }
    }
}
