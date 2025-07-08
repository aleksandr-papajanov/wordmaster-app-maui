using DynamicData;
using WordMaster.Data.Models;
using WordMasterApp.EntityViewModels.Actions;

namespace WordMasterApp.EntityViewModels
{
    public class DeckEntityViewModel : EntityViewModelBase<Deck>
    {
        private DeckEntityViewModelActions? _actions;
        public DeckEntityViewModelActions Actions
        {
            get => _actions ?? throw new InvalidOperationException("Actions must be set before using this view model.");
            set { _actions = value; }
        }

        // UI read-only properties
        public Guid Id { get; private set; }
        public string Name { get; set; } = string.Empty;

        // Navigation properties
        public LanguageEntityViewModel SourceLanguage => Actions.GetSourceLanguage();
        public LanguageEntityViewModel TargetLanguage => Actions.GetTargetLanguage();

        public IObservable<IChangeSet<WordEntityViewModel>> Words => Actions.GetWordsStream("");


        public DeckEntityViewModel(Deck entity) : base(entity) { }


        protected override Dictionary<string, UpdatePropertyMap> GetUpdateMap() => new()
        {
            {
                nameof(Deck.Id),
                new (
                    () => Id = _entity.Id,
                    () => _entity.Id = Id)
            },
            {
                nameof(Deck.Name),
                new (
                    () => Name = _entity.Name,
                    () => _entity.Name = Name)
            }
        };
    }
}
