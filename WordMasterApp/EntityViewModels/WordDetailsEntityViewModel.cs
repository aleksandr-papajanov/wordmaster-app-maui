using WordMaster.Common;
using WordMaster.Data.Models;
using WordMasterApp.EntityViewModels.Actions;

namespace WordMasterApp.EntityViewModels
{
    public class WordDetailsEntityViewModel : EntityViewModelBase<WordDetails>
    {
        private WordDetailsEntityViewModelActions? _actions;
        public WordDetailsEntityViewModelActions Actions
        {
            get => _actions ?? throw new InvalidOperationException("Actions must be set before using this view model.");
            set { _actions = value; }
        }

        // UI read-only properties
        public string Pronunciation { get; set; } = string.Empty;
        public string PartOfSpeechLabel { get; set; } = string.Empty;
        public PartOfSpeech PartOfSpeechType { get; set; }

        // Read-only properties


        //Navigation properties
        public WordEntityViewModel? Word => Actions.GetParentWord();


        public WordDetailsEntityViewModel(WordDetails entity) : base(entity) { }

        
        protected override Dictionary<string, UpdatePropertyMap> GetUpdateMap() => new()
        {
            {
                nameof(WordDetails.Pronunciation),
                new (
                    () => Pronunciation = _entity.Pronunciation ?? string.Empty,
                    () => _entity.Pronunciation = Pronunciation)
            },
            {
                nameof(WordDetails.PartOfSpeechLabel),
                new (
                    () => PartOfSpeechLabel = _entity.PartOfSpeechLabel ?? string.Empty,
                    () => _entity.PartOfSpeechLabel = PartOfSpeechLabel)
            },
            {
                nameof(WordDetails.PartOfSpeechType),
                new (
                    () => PartOfSpeechType = (PartOfSpeech)_entity.PartOfSpeechType,
                    () => _entity.PartOfSpeechType = (int)PartOfSpeechType)
            },
        };


        public async Task SaveAsync()
        {
            if (IsManaged)
            {
                await Actions.UpdateAsync((t) =>
                {
                    UpdateEntity();
                });
            }
            else
            {
                UpdateEntity();

                await Actions.CreateAsync();
            }
        }

        public async Task DeleteAsync()
        {
            await Actions.DeleteAsync();
        }
    }
}


