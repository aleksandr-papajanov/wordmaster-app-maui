using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using WordMasterApp.DataLayer.Models;
using WordMasterApp.Features.MessageContainer;
using WordMasterApp.Services;
using WordMasterApp.Services.Generation;
using WordMasterApp.Services.Generation.Stages;

namespace WordMasterApp.EntityWrappers
{
    public class WordWrapper : EntityWrapperBase<Word>, IValidatableViewModel
    {
        private readonly IWordService _wordService;

        //Navigation property to the associated Word, if any
        private DeckWrapper? _deck;
        public DeckWrapper? Deck => _deck ??= _wordService.GetDeck(_entity);

        // ViewModel properties
        private Guid _id;
        public Guid Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

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

        private string _pronunciation = string.Empty;
        public string Pronunciation
        {
            get => _pronunciation;
            set => this.RaiseAndSetIfChanged(ref _pronunciation, value);
        }

        private string _partOfSpeechLabel = string.Empty;
        public string PartOfSpeechLabel
        {
            get => _partOfSpeechLabel;
            set => this.RaiseAndSetIfChanged(ref _partOfSpeechLabel, value);
        }

        private PartOfSpeech _partOfSpeechType;
        public PartOfSpeech PartOfSpeechType
        {
            get => _partOfSpeechType;
            set => this.RaiseAndSetIfChanged(ref _partOfSpeechType, value);
        }

        private string _synonyms = string.Empty;
        public string Synonyms
        {
            get => _synonyms;
            set => this.RaiseAndSetIfChanged(ref _synonyms, value);
        }

        private string _antonyms = string.Empty;
        public string Antonyms
        {
            get => _antonyms;
            set => this.RaiseAndSetIfChanged(ref _antonyms, value);
        }

        private WordUsageFrequenсy _usageFrequency;
        public WordUsageFrequenсy UsageFrequency
        {
            get => _usageFrequency;
            set => this.RaiseAndSetIfChanged(ref _usageFrequency, value);
        }

        public DateTimeOffset CreatedAt => _entity.CreatedAt;

        // Implementing IValidatableViewModel
        public IValidationContext ValidationContext { get; } = new ValidationContext();

        private ObservableAsPropertyHelper<bool> _isValid;
        public bool IsValid => _isValid.Value;
        public IObservable<bool> IsValidChanged => ValidationContext.WhenAnyValue(x => x.IsValid);

        public WordWrapper(DeckWrapper deck, IWordService wordService) : base(new Word())
        {
            if (!deck.IsManaged)
            {
                throw new Exception("Can not create wrapper of detouched entity");
            }

            _wordService = wordService;

            _entity.DeckId = deck.Id;

            BindProperties();
            SetupValidation();
        }

        public WordWrapper(Word entity, IWordService wordService) : base(entity)
        {
            if (!entity.IsManaged)
            {
                throw new Exception("Can not create wrapper of detouched entity");
            }

            _wordService = wordService;

            Id = _entity.Id;
            Text = _entity.Text;
            Translation = _entity.Translation;
            Definition = _entity.Definition ?? string.Empty;
            Pronunciation = _entity.Pronunciation ?? string.Empty;
            PartOfSpeechLabel = _entity.PartOfSpeechLabel ?? string.Empty;
            PartOfSpeechType = (PartOfSpeech)_entity.PartOfSpeechType;
            Synonyms = _entity.Synonyms ?? string.Empty;
            Antonyms = _entity.Antonyms ?? string.Empty;
            UsageFrequency = (WordUsageFrequenсy)_entity.UsageFrequency;

            BindProperties();
            SetupValidation();
        }

        private void BindProperties()
        {
            var actions = new Dictionary<string, Action>
            {
                { nameof(Word.Id),                () => Id = _entity.Id },
                { nameof(Word.Text),              () => Text = _entity.Text },
                { nameof(Word.Translation),       () => Translation = _entity.Translation },
                { nameof(Word.Definition),        () => Definition = _entity.Definition ?? string.Empty },
                { nameof(Word.Pronunciation),     () => Pronunciation = _entity.Pronunciation ?? string.Empty },
                { nameof(Word.PartOfSpeechLabel), () => PartOfSpeechLabel = _entity.PartOfSpeechLabel ?? string.Empty },
                { nameof(Word.PartOfSpeechType),  () => PartOfSpeechType = (PartOfSpeech)_entity.PartOfSpeechType },
                { nameof(Word.Synonyms),          () => Synonyms = _entity.Synonyms ?? string.Empty },
                { nameof(Word.Antonyms),          () => Antonyms = _entity.Antonyms ?? string.Empty },
                { nameof(Word.UsageFrequency),    () => UsageFrequency = (WordUsageFrequenсy)_entity.UsageFrequency }
            };

            _entity.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName != null && actions.TryGetValue(e.PropertyName, out var action))
                {
                    action.Invoke();
                }
            };
        }

        public ValidationHelper TextValidationHelper { get; private set; }
        public ValidationHelper TranslationValidationHelper { get; private set; }

        private void SetupValidation()
        {
            ValidationContext
                .WhenAnyValue(x => x.IsValid)
                .ToProperty(this, x => x.IsValid, out _isValid);

            TextValidationHelper = this.ValidationRule(
                vm => vm.Text,
                text => !string.IsNullOrWhiteSpace(text),
                "Word cannot be empty"
            );

            TranslationValidationHelper = this.ValidationRule(
                vm => vm.Translation,
                text => !string.IsNullOrWhiteSpace(text),
                "Translation cannot be empty"
            );
        }


        public async Task SaveAsync()
        {
            if (IsManaged)
            {
                await _wordService.UpdateAsync(_entity, (t) =>
                {
                    _entity.Text = Text;
                    _entity.Translation = Translation;
                    _entity.Definition = Definition;
                    _entity.Pronunciation = Pronunciation;
                    _entity.PartOfSpeechLabel = PartOfSpeechLabel;
                    _entity.PartOfSpeechType = (int)PartOfSpeechType;
                    _entity.Synonyms = Synonyms;
                    _entity.Antonyms = Antonyms;
                    _entity.UsageFrequency = (int)UsageFrequency;
                });
            }
            else
            {
                _entity.Text = Text;
                _entity.Translation = Translation;
                _entity.Definition = Definition;
                _entity.Pronunciation = Pronunciation;
                _entity.PartOfSpeechLabel = PartOfSpeechLabel;
                _entity.PartOfSpeechType = (int)PartOfSpeechType;
                _entity.Synonyms = Synonyms;
                _entity.Antonyms = Antonyms;
                _entity.UsageFrequency = (int)UsageFrequency;

                await _wordService.CreateAsync(_entity);
            }
        }

        public async Task DeleteAsync()
        {
            await _wordService.DeleteAsync(_entity);
        }

        public async Task AutoCompleteAsync(Action<MessageModel> publishMessage)
        {
            if (IsManaged)
            {
                throw new InvalidOperationException("Cannot auto-complete a managed entity.");
            }

            var session = _wordService.AutoComplete(this);

            session.OnInteractionRequested += async interaction =>
            {
                if (interaction.StageName == nameof(WordCompletionStage))
                {
                    var box = new ConfirmWordCompletionMessage(
                        interaction.ContextSnapshot.Get<string>(WordCompletionKeys.Word),
                        interaction.ContextSnapshot.Get<string>(WordCompletionKeys.Translation),
                        interaction.ContextSnapshot.Get<string>(WordCompletionKeys.Definition)
                    );

                    publishMessage(box);

                    var result = await box.Completion;

                    if (result is ConfirmWordCompletionMessageResult.Yes)
                    {
                        await interaction.SessionControl.Continue();
                    }
                    else if (result is ConfirmWordCompletionMessageResult.TryAgain)
                    {
                        await interaction.SessionControl.Repeat();
                    }
                    else if (result is ConfirmWordCompletionMessageResult.Cancel)
                    {
                        await interaction.SessionControl.Cancel();
                    }
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
                    Text              = context.Get<string>(WordCompletionKeys.Word);
                    Translation       = context.Get<string>(WordCompletionKeys.Translation);
                    Definition        = context.Get<string>(WordCompletionKeys.Definition);
                    Pronunciation     = context.Get<string>(WordCompletionKeys.Pronunciation);
                    PartOfSpeechLabel = context.Get<string>(WordCompletionKeys.PartOfSpeechLabel);
                    PartOfSpeechType  = context.Get<PartOfSpeech>(WordCompletionKeys.PartOfSpeechType);
                    Synonyms          = string.Join(", ", context.Get<IEnumerable<string>>(WordCompletionKeys.Synonyms));
                    Antonyms          = string.Join(", ", context.Get<IEnumerable<string>>(WordCompletionKeys.Antonyms));
                    UsageFrequency    = context.Get<WordUsageFrequenсy>(WordCompletionKeys.UsageFrequency);
                }

                await Task.CompletedTask;
            };

            await session.RunAsync();
        }
    }
}
