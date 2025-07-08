using DynamicData;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using WordMaster.Common;
using WordMaster.Data.Models;
using WordMaster.Generation;
using WordMaster.Generation.DTOs;
using WordMaster.Generation.Interfaces;
using WordMaster.Generation.Stages;
using WordMasterApp.EntityViewModels.Actions;
using WordMasterApp.Features.MessageContainer.Messages;
using WordMasterApp.Helpers;

namespace WordMasterApp.EntityViewModels
{
    public class WordEntityViewModel : EntityViewModelBase<Word>
    {
        private readonly ISessionBuilder _generationSessionBuilder;

        private WordEntityViewModelActions? _actions;
        public WordEntityViewModelActions Actions
        {
            get => _actions ?? throw new InvalidOperationException("Actions must be set before using this view model.");
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
        public WordUsageFrequenсy UsageFrequency { get; set; }

        // Read-only properties
        public DateTimeOffset CreatedAt => _entity.CreatedAt;

        // Navigation properties
        public DeckEntityViewModel Deck => Actions.GetParentDeck();
        public WordDetailsEntityViewModel? Details => Actions.GetDetails();
        public IObservable<IChangeSet<WordUsageEntityViewModel>> Usages(string filter) => Actions.GetUsagesStream("");
        public IObservable<IChangeSet<WordRelationEntityViewModel>> Synonyms => Actions.GetRelatedWordsStream(WordRelationType.Synonym);
        public IObservable<IChangeSet<WordRelationEntityViewModel>> Antonyms => Actions.GetRelatedWordsStream(WordRelationType.Antonym);


        public WordEntityViewModel(Word entity, ISessionBuilder generationSessionBuilder) : base(entity)
        {
            _generationSessionBuilder = generationSessionBuilder;
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
                    () => Text = _entity.Text ?? string.Empty,
                    () => _entity.Text = Text)
            },
            {
                nameof(Word.Translation),
                new (
                    () => Translation = _entity.Translation ?? string.Empty,
                    () => _entity.Translation = Translation)
            },
            {
                nameof(Word.Definition),
                new (
                    () => Definition = _entity.Definition ?? string.Empty,
                    () => _entity.Definition = Definition)
            },
            {
                nameof(Word.UsageFrequency),
                new (
                    () => UsageFrequency = (WordUsageFrequenсy)(_entity.UsageFrequency ?? 0),
                    () => _entity.UsageFrequency = (int)UsageFrequency)
            },
            {
                nameof(Word.Details),
                new (
                    () => this.RaisePropertyChanged(nameof(Details)),
                    () => { /* It can't be changed from UI or ViewModel directly */ })
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

        public async Task DeleteAsync(DeckEntityViewModel deck)
        {
            await Actions.DeleteAsync(deck);
        }

        public async Task AiCompleteAsync(
            Interaction<IContext, ConfirmRelatedWordCompletionMessageResult> basicsInteraction,
            Interaction<IContext, ConfirmRelatedWordUsagesCompletionMessageResult> usagesInteraction)
        {
            var request = new WordCompletionRequest
            {
                Word = Text,
                SourceLanguage = Deck.SourceLanguage.Name,
                TargetLanguage = Deck.TargetLanguage.Name,
                UsagesCount = 3,
            };

            _generationSessionBuilder.Reset();

            var session = _generationSessionBuilder
                .UseMiddleware(new LoadingIndicationGenerationMiddleware())
                .UseRetryMiddleware(1)
                .UseWordBasicsCompletionStage()
                .UseStage(null, new InteractionStage<ConfirmRelatedWordCompletionMessageResult>(
                    interaction: basicsInteraction,
                    onResult: async (context, control, result) =>
                    {
                        switch (result)
                        {
                            case ConfirmRelatedWordCompletionMessageResult.TryAgain:
                                await control.RepeatStage();
                                break;

                            case ConfirmRelatedWordCompletionMessageResult.AddToDeck:
                                await control.ToNextStage();
                                break;

                            case ConfirmRelatedWordCompletionMessageResult.Save:
                                await control.FinishAfter("save");
                                await control.ToNextStage();
                                break;

                            case ConfirmRelatedWordCompletionMessageResult.Cancel:
                            default:
                                await control.Cancel();
                                break;
                        }
                    }))
                .UseCustomActionStage("save", async (context, control) =>
                {
                    // Save progress
                    var response = context.ToWordBasicsCompletionResponse();

                    Text = response.Word;
                    Translation = response.Translation;
                    Definition = response.Definition;
                    UsageFrequency = response.UsageFrequency;

                    await SaveAsync();
                })
                .UseWordDetailsCompletionStage()
                .UseCustomActionStage(null, async (context, control) =>
                {
                    // Save progress
                    var details = context.ToWordCompletionResponse();

                    await Actions.CreateDetails(
                        details.Pronunciation,
                        details.PartOfSpeechLabel,
                        details.PartOfSpeechType);

                    foreach (var synonym in details.Synonyms)
                    {
                        await Actions.CreateRelation(synonym, WordRelationType.Synonym);
                    }

                    foreach (var antonym in details.Antonyms)
                    {
                        await Actions.CreateRelation(antonym, WordRelationType.Antonym);
                    }
                })
                .UseWordUsageCompletionStage()
                .UseStage(null, new InteractionStage<ConfirmRelatedWordUsagesCompletionMessageResult>(
                    interaction: usagesInteraction,
                    onResult: async (context, control, result) =>
                    {
                        switch (result)
                        {
                            case ConfirmRelatedWordUsagesCompletionMessageResult.TryAgain:
                                await control.RepeatStage();
                                break;

                            case ConfirmRelatedWordUsagesCompletionMessageResult.AddToDeck:
                                await control.ToNextStage();
                                break;

                            case ConfirmRelatedWordUsagesCompletionMessageResult.Cancel:
                            default:
                                await control.Cancel();
                                break;
                        }
                    }))
                .UseCustomActionStage(null, async (context, control) =>
                {
                    // Save progress
                    var usages = context.ToWordCompletionResponse();
                    foreach (var usage in usages.Sentences)
                    {
                        await Actions.CreateUsage(usage.Key, usage.Value);
                    }
                })
                .SetRequest(request)
                .Build();

            session.OnComplete += async context =>
            {
                switch (context.SessionResult)
                {
                    case SessionResult.Cancelled:
                        break;

                    case SessionResult.Error:
                        throw new Exception(context.ErrorMessage ?? "An error occurred during word completion.");

                    case SessionResult.Success:
                        break;

                    default:
                        throw new NotImplementedException();
                }
            };

            await session.RunAsync();
        }
    }
}
