using DynamicData;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using WordMaster.Generation;
using WordMaster.Generation.DTOs;
using WordMaster.Generation.Interfaces;
using WordMasterApp.EntityViewModels;
using WordMasterApp.Features.MessageContainer.Interfaces;
using WordMasterApp.Features.MessageContainer.Messages;
using WordMasterApp.Helpers;

namespace WordMasterApp.Features.RelatedWords
{
    public class RelatedWordsViewViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly IMessageService _messageService;
        private readonly ISessionBuilder _generator;

        // Current word
        private readonly ObservableAsPropertyHelper<WordEntityViewModel?> _currentWord;
        public WordEntityViewModel? CurrentWord => _currentWord.Value;

        // UI collections
        private ReadOnlyObservableCollection<WordRelationEntityViewModel> _synonyms = null!;
        public ReadOnlyObservableCollection<WordRelationEntityViewModel> Synonyms => _synonyms;

        private ReadOnlyObservableCollection<WordRelationEntityViewModel> _antonyms = null!;
        public ReadOnlyObservableCollection<WordRelationEntityViewModel> Antonyms => _antonyms;

        // Commands
        public ICommand ExplainWordCommand { get; private set; } = null!;

        // Implementing IActivatableViewModel
        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public RelatedWordsViewViewModel(IObservable<WordEntityViewModel?> word, IMessageService messageService, ISessionBuilder generator)
        {
            _messageService = messageService;
            _generator = generator;

            word.ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.CurrentWord, out _currentWord);

            SetupCommands();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.CurrentWord)
                    .Select(x => x == null || !x.IsManaged
                        ? Observable.Return(ChangeSet<WordRelationEntityViewModel>.Empty)
                        : x.Synonyms)
                    .Switch()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out _synonyms)
                    .Subscribe(_ =>
                    {
                        this.RaisePropertyChanged(nameof(Synonyms));
                    })
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.CurrentWord)
                    .Select(x => x == null || !x.IsManaged
                        ? Observable.Return(ChangeSet<WordRelationEntityViewModel>.Empty)
                        : x.Antonyms)
                    .Switch()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out _antonyms)
                    .Subscribe(_ => this.RaisePropertyChanged(nameof(Antonyms)))
                    .DisposeWith(disposables);
            });
        }


        private void SetupCommands()
        {
            var word = this.WhenAnyValue(x => x.CurrentWord).Select(x => x != null);

            ExplainWordCommand = ReactiveCommand.CreateFromTask<WordRelationEntityViewModel>(ExplainRelatedWordAsync, word);
        }

        private async Task ExplainRelatedWordAsync(WordRelationEntityViewModel relatedWord)
        {
            if (relatedWord == null || CurrentWord == null || relatedWord.ParentWordId != CurrentWord.Id)
                return;

            if (relatedWord.IsCompleted)
            {
                //await ShowRelatedWordBox(relatedWord);
            }
            else
            {
                // If the word is not completed, we need to complete it first
                //await CompleteWord(relatedWord);
            }
        }

        //private async Task ShowRelatedWordBox(WordRelationEntityViewModel relatedWord)
        //{
        //    var box = new ConfirmRelatedWordBasicCompletionMessage(
        //        relatedWord.RelatedWord.Text,
        //        relatedWord.RelatedWord.Translation ?? throw new Exception("Translation is not set for the related word."),
        //        relatedWord.RelatedWord.Definition ?? throw new Exception("Definition is not set for the related word."),
        //        WordMaster.Common.WordUsageFrequenсy.Common
        //        //relatedWord.RelatedWord.UsageFrequency ?? throw new Exception("Usage frequency is not set for the related word.")
        //    );

        //    _messageService.Publish(box);

        //    // Handle the result of the confirmation box
        //    Func<Task> action = await box.Completion switch
        //    {
        //        ConfirmRelatedWordCompletionMessageResult.TryAgain => async () =>
        //        {
        //            await CompleteWord(relatedWord);
        //        },
        //        ConfirmRelatedWordCompletionMessageResult.AddToDeck => async () =>
        //        {
        //            await CompleteWord(relatedWord);
        //        },
        //        _ => async () =>
        //        {
        //            // On save or cancel, just update the entity
        //            await relatedWord.SaveAsync();
        //        }
        //    };

        //    await action.Invoke();
        //}
    }
}
