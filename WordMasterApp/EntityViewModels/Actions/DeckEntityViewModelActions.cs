using DynamicData;
using Realms;

namespace WordMasterApp.EntityViewModels.Actions
{
    public class DeckEntityViewModelActions : EntityViewModelActionsBase
    {
        public required Func<LanguageEntityViewModel> GetSourceLanguage { get; set; }
        public required Func<LanguageEntityViewModel> GetTargetLanguage { get; set; }
        public required Func<string, IObservable<IChangeSet<WordEntityViewModel>>> GetWordsStream { get; set; }
    }
}
