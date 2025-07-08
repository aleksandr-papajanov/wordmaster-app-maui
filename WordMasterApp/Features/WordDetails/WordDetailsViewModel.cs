using DynamicData;
using Microsoft.Maui.Controls;
using Microsoft.VisualBasic;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using WordMaster.Common;
using WordMaster.Generation;
using WordMaster.Generation.DTOs;
using WordMaster.Generation.Interfaces;
using WordMaster.Generation.Stages;
using WordMasterApp.EntityViewModels;
using WordMasterApp.Features.MessageContainer.Interfaces;
using WordMasterApp.Features.MessageContainer.Messages;
using WordMasterApp.Features.RelatedWords;
using WordMasterApp.Features.WordUsage;
using WordMasterApp.Messages;
using static System.Net.Mime.MediaTypeNames;

namespace WordMasterApp.Features
{
    public partial class WordDetailsViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly IMessageService _messageService;

        private readonly ObservableAsPropertyHelper<WordEntityViewModel?> _currentWord;
        public WordEntityViewModel? CurrentWord => _currentWord.Value;

        private bool _hasTriedToUpdate = false;
        public bool HasTriedToUpdate
        {
            get => _hasTriedToUpdate;
            private set => this.RaiseAndSetIfChanged(ref _hasTriedToUpdate, value);
        }

        // ViewModels
        public WordUsageViewViewModel WordUsageViewModel { get; }
        public RelatedWordsViewViewModel RelatedWordsViewModel { get; }

        // Commands
        public ICommand UpdateCommand { get; private set; }
        public ICommand AutoCompleteCommand { get; private set; }

        // Interaction for stages
        public Interaction<IContext, ConfirmRelatedWordCompletionMessageResult> WordBasicsConfirmationInteraction { get; } = new();
        public Interaction<IContext, ConfirmRelatedWordUsagesCompletionMessageResult> WordUsagesConfirmationInteraction { get; } = new();

        // Implementing IActivatableViewModel
        public ViewModelActivator Activator { get; } = new ViewModelActivator();


        public WordDetailsViewModel(IObservable<WordEntityViewModel?> word,
                                    IMessageService messageService,
                                    IWordUsageViewViewModelFactory wordUsageFactory,
                                    IRelatedWordsViewViewModelFactory relatedWordsFactory)
        {
            _messageService = messageService;

            word.ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.CurrentWord, out _currentWord);

            SetupCommands();

            // important to create the view models in constructor for propper binding
            WordUsageViewModel = wordUsageFactory.Create(word);
            RelatedWordsViewModel = relatedWordsFactory.Create(word);

            this.WhenActivated(disposables =>
            {
                word.Subscribe(word =>
                {
                    HasTriedToUpdate = false;
                })
                .DisposeWith(disposables);

                WordBasicsConfirmationInteraction.RegisterHandler(async interaction =>
                {
                    var request = interaction.Input;
                    var wordBasics = request.ToWordBasicsCompletionResponse();

                    var box = new ConfirmRelatedWordBasicCompletionMessage(
                        wordBasics.Word,
                        wordBasics.Translation,
                        wordBasics.Definition,
                        wordBasics.UsageFrequency);

                    _messageService.Publish(box);

                    var result = (ConfirmRelatedWordCompletionMessageResult?)await box.Completion
                        ?? throw new InvalidOperationException("Confirmation result cannot be null.");

                    interaction.SetOutput(result);
                });

                WordUsagesConfirmationInteraction.RegisterHandler(async interaction =>
                {
                    var request = interaction.Input;
                    var wordUsages = request.ToWordUsagesCompletionResponse();

                    if (usagesBox == null)
                    {
                        usagesBox = new ConfirmRelatedWordUsagesCompletionMessage(wordUsages.Word, wordUsages.Sentences);
                        usagesBox.CloseOnAction = false;

                        _messageService.Publish(usagesBox);
                    }
                    else
                    {
                        usagesBox.AddSentencesFromDictionary(wordUsages.Sentences);
                    }

                    var result = (ConfirmRelatedWordUsagesCompletionMessageResult?)await usagesBox.Completion
                        ?? throw new InvalidOperationException("Confirmation result cannot be null.");

                    switch (result)
                    {
                        case ConfirmRelatedWordUsagesCompletionMessageResult.TryAgain:
                            usagesBox.RemoveUnselectedSentences();
                            break;

                        case ConfirmRelatedWordUsagesCompletionMessageResult.AddToDeck:
                            var sentences = usagesBox.GetSelectedSentencesAsDictionary();
                            request.Set(nameof(WordCompletionResponse.Sentences), sentences);
                            usagesBox.Close();
                            break;

                        case ConfirmRelatedWordUsagesCompletionMessageResult.Cancel:
                        default:
                            usagesBox.Close();
                            break;
                    }

                    interaction.SetOutput(result);
                });
            });
        }


        private void SetupCommands()
        {
            var word  = this.WhenAnyValue(x => x.CurrentWord).Select(x => x != null);
            var isNew = this.WhenAnyValue(x => x.CurrentWord).Select(x => !(x == null || x.IsManaged));
            var tried = this.WhenAnyValue(x => x.HasTriedToUpdate);
            var valid = this.WhenAnyValue(x => x.CurrentWord).Select(x => x != null ? x.IsValidChanged : Observable.Return(false)).Switch();

            var canUpdate = Observable.CombineLatest(word, tried, valid, (word, tried, valid) => word && (tried ? valid : true));
            //var canComplete = Observable.CombineLatest(managed, (managed) => !managed);

            UpdateCommand = ReactiveCommand.CreateFromTask(UpdateWordAsync, canUpdate);
            AutoCompleteCommand = ReactiveCommand.CreateFromTask(AiCompleteAsync, isNew);
            
        }

        private async Task AiCompleteAsync()
        {
            if (CurrentWord == null)
                return;

            await CurrentWord.AiCompleteAsync(WordBasicsConfirmationInteraction, WordUsagesConfirmationInteraction);
        }


        private ConfirmRelatedWordUsagesCompletionMessage? usagesBox = null;


        private async Task ConfirmUsages(WordCompletionResponse response, StageInteractionRequest interaction)
        {
            if (usagesBox == null)
            {
                usagesBox = new ConfirmRelatedWordUsagesCompletionMessage(response.Word, response.Sentences);
                usagesBox.CloseOnAction = false;

                _messageService.Publish(usagesBox);
            }
            else
            {
                usagesBox.AddSentencesFromDictionary(response.Sentences);
            }

            // Handle the result of the confirmation box
            Func<Task> action = await usagesBox.Completion switch
            {
                ConfirmRelatedWordUsagesCompletionMessageResult.TryAgain => async () =>
                {
                    usagesBox.RemoveUnselectedSentences();
                    await interaction.SessionControl.RepeatStage();
                }
                ,
                ConfirmRelatedWordUsagesCompletionMessageResult.AddToDeck => async () =>
                {
                    response.Sentences = usagesBox.GetSelectedSentencesAsDictionary();

                    // to next stage
                    usagesBox.Close();
                    await interaction.SessionControl.ToNextStage();
                }
                ,
                _ => async () =>
                {
                    usagesBox.Close();
                    await interaction.SessionControl.Cancel();
                }
            };

            await action();
        }

        private async Task UpdateWordAsync()
        {
            if (CurrentWord == null)
                return;

            if (CurrentWord.IsValid)
            {
                await CurrentWord.SaveAsync();

                if (CurrentWord.IsManaged)
                {
                    HasTriedToUpdate = false;
                }
            }
            else
            {
                HasTriedToUpdate = true;
            }
        }
    }
}