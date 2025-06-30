using DynamicData;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using WordMaster.Common;
using WordMaster.Data.Models;
using WordMasterApp.EntityViewModels.Actions;
using WordMasterApp.Features.MessageContainer;
using WordMaster.Generation;
using WordMaster.Generation.DTOs;
using WordMaster.Generation.Stages;

namespace WordMasterApp.EntityViewModels
{
    public class WordEntityViewModel : EntityViewModelBase<Word>
    {
        private WordEntityViewModelActions _actions;
        public WordEntityViewModelActions Actions
        {
            set { _actions = value; }
        }

        // UI properties
        private string _text = string.Empty;
        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }

        private string _translation = string.Empty;
        public string Translation
        {
            get => _translation;
            set => this.RaiseAndSetIfChanged(ref _translation, value);
        }

        private string _definition = string.Empty;
        public string Definition
        {
            get => _definition;
            set => this.RaiseAndSetIfChanged(ref _definition, value);
        }

        // UI read-only properties
        public Guid Id { get; set; }
        public string Pronunciation { get; set; } = string.Empty;
        public string PartOfSpeechLabel { get; set; } = string.Empty;
        public PartOfSpeech PartOfSpeechType { get; set; }
        public WordUsageFrequenсy UsageFrequency { get; set; }

        // Read-only properties
        public DateTimeOffset CreatedAt => _entity.CreatedAt;

        // Navigation properties
        public DeckEntityViewModel? Deck => _actions.GetParentDeck();
        public IObservable<IChangeSet<WordUsageEntityViewModel>> Usages(string filter) => _actions.GetUsagesStream("");
        public IObservable<IChangeSet<RelatedWordEntityViewModel>> Synonyms => _actions.GetRelatedWordsStream(WordRelationType.Synonym);
        public IObservable<IChangeSet<RelatedWordEntityViewModel>> Antonyms => _actions.GetRelatedWordsStream(WordRelationType.Antonym);


        public WordEntityViewModel(Word entity) : base(entity)
        {
            SetupValidation();
        }

        protected override Dictionary<string, UpdatePropertyMap> GetUpdateMap() => new()
        {
            {
                nameof(Word.Id),
                new (
                    () => Id = _entity.Id,
                    () => _entity.Id = Id)
            },
            {
                nameof(Word.Text),
                new (
                    () => Text = _entity.Text,
                    () => _entity.Text = Text)
            },
            {
                nameof(Word.Translation),
                new (
                    () => Translation = _entity.Translation,
                    () => _entity.Translation = Translation)
            },
            {
                nameof(Word.Definition),
                new (
                    () => Definition = _entity.Definition ?? string.Empty,
                    () => _entity.Definition = Definition)
            },
            {
                nameof(Word.Pronunciation),
                new (
                    () => Pronunciation = _entity.Pronunciation ?? string.Empty,
                    () => _entity.Pronunciation = Pronunciation)
            },
            {
                nameof(Word.PartOfSpeechLabel),
                new (
                    () => PartOfSpeechLabel = _entity.PartOfSpeechLabel ?? string.Empty,
                    () => _entity.PartOfSpeechLabel = PartOfSpeechLabel)
            },
            {
                nameof(Word.PartOfSpeechType),
                new (
                    () => PartOfSpeechType = (PartOfSpeech)_entity.PartOfSpeechType,
                    () => _entity.PartOfSpeechType = (int)PartOfSpeechType)
            },
            {
                nameof(Word.UsageFrequency),
                new (
                    () => UsageFrequency = (WordUsageFrequenсy)_entity.UsageFrequency,
                    () => _entity.UsageFrequency = (int)UsageFrequency)
            }
        };

        protected override void SetupValidation()
        {
            base.SetupValidation();

            this.ValidationRule(
                vm => vm.Text,
                text => !string.IsNullOrWhiteSpace(text),
                "Word cannot be empty"
            );

            this.ValidationRule(
                vm => vm.Translation,
                text => !string.IsNullOrWhiteSpace(text),
                "Translation cannot be empty"
            );
        }


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

        public async Task AutoCompleteAsync(Action<MessageModel> publishMessage)
        {
            if (IsManaged)
            {
                throw new InvalidOperationException("Cannot auto-complete a managed entity.");
            }

            var request = new WordCompletionRequest
            {
                Word = Text,
                Translation = Translation,
                Definition = Definition,
                SourceLanguage = Deck?.SourceLanguage?.Name ?? throw new Exception("Source language is not set for the current deck."),
                TargetLanguage = Deck?.TargetLanguage?.Name ?? throw new Exception("Target language is not set for the current deck."),
            };

            var session = _actions.CompleteWordFull(request);

            session.OnInteractionRequested += async interaction =>
            {
                if (interaction.StageName == nameof(WordBaseCompletionStage))
                {
                    var box = new ConfirmWordCompletionMessage(
                        interaction.ContextSnapshot.Get<string>(WordBaseCompletionKeys.Word),
                        interaction.ContextSnapshot.Get<string>(WordBaseCompletionKeys.Translation),
                        interaction.ContextSnapshot.Get<string>(WordBaseCompletionKeys.Definition),
                        interaction.ContextSnapshot.Get<WordUsageFrequenсy>(WordBaseCompletionKeys.UsageFrequency)
                    );

                    publishMessage(box);

                    var result = await box.Completion;

                    Func<Task> action = result switch
                    {
                        ConfirmWordCompletionMessageResult.Yes => interaction.SessionControl.Continue,
                        ConfirmWordCompletionMessageResult.TryAgain => interaction.SessionControl.Repeat,
                        ConfirmWordCompletionMessageResult.Cancel => interaction.SessionControl.Cancel,
                        _ => throw new InvalidOperationException("Unexpected result from confirmation message.")
                    };
                }
            };

            session.OnComplete += async context =>
            {
                if (context.SessionResult == GenerationSessionResult.Error)
                {
                    publishMessage(new ErrorMessage(context.ErrorMessage ?? "An error occurred during word completion."));
                }
                else if (context.SessionResult == GenerationSessionResult.Success)
                { 
                    Text              = context.Get<string>(WordBaseCompletionKeys.Word);
                    Translation       = context.Get<string>(WordBaseCompletionKeys.Translation);
                    Definition        = context.Get<string>(WordBaseCompletionKeys.Definition);
                    Pronunciation     = context.Get<string>(WordBaseCompletionKeys.Pronunciation);
                    PartOfSpeechLabel = context.Get<string>(WordBaseCompletionKeys.PartOfSpeechLabel);
                    PartOfSpeechType  = context.Get<PartOfSpeech>(WordBaseCompletionKeys.PartOfSpeechType);
                    //Synonyms          = string.Join(", ", context.Get<IEnumerable<string>>(WordCompletionKeys.Synonyms));
                    //Antonyms          = string.Join(", ", context.Get<IEnumerable<string>>(WordCompletionKeys.Antonyms));
                    UsageFrequency    = context.Get<WordUsageFrequenсy>(WordBaseCompletionKeys.UsageFrequency);
                }

                await Task.CompletedTask;
            };

            await session.RunAsync();
        }
    }
}
