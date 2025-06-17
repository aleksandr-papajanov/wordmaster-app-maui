using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using WordMasterApp.DIFactories;
using WordMasterApp.EntityWrappers;
using WordMasterApp.Features.MessageContainer;
using WordMasterApp.Messages;
using WordMasterApp.Services;
using WordMasterApp.Services.Generation;

namespace WordMasterApp.Features
{
    public partial class WordDetailsViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly IWordService _wordService;
        private readonly IGenerationService _aiService;
        private readonly IMessageService _messageService;
        private readonly IWordUsageViewModelFactory _wordUsageFactory;

        private readonly ObservableAsPropertyHelper<WordWrapper?> _currentWord;
        public WordWrapper? CurrentWord => _currentWord.Value;

        private bool _hasTriedToUpdate = false;
        public bool HasTriedToUpdate
        {
            get => _hasTriedToUpdate;
            private set => this.RaiseAndSetIfChanged(ref _hasTriedToUpdate, value);
        }

        // ViewModels
        public WordUsageViewViewModel WordUsageViewModel { get; }

        // Commands
        public ICommand UpdateCommand { get; private set; }
        public ICommand AutoCompleteCommand { get; private set; }


        // Implementing IActivatableViewModel
        public ViewModelActivator Activator { get; } = new ViewModelActivator();


        public WordDetailsViewModel(IObservable<WordWrapper?> word,
                                    IWordService wordService,
                                    IGenerationService aiService,
                                    IMessageService messageService,
                                    IWordUsageViewModelFactory wordUsageFactory)
        {
            _wordService = wordService;
            _aiService = aiService;
            _messageService = messageService;
            _wordUsageFactory = wordUsageFactory;

            word.ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.CurrentWord, out _currentWord);

            //word.ObserveOn(RxApp.MainThreadScheduler)
            //    .Subscribe(x => CurrentWord = x);

            SetupCommands();

            // important to create the view models in constructor for propper binding
            WordUsageViewModel = wordUsageFactory.Create(word);

            this.WhenActivated(disposables =>
            {
                word.Subscribe(word =>
                {
                    HasTriedToUpdate = false;
                })
                .DisposeWith(disposables);
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
            AutoCompleteCommand = ReactiveCommand.CreateFromTask(AutoCompleteAsync, isNew);
        }

        private async Task AutoCompleteAsync()
        {
            if (CurrentWord == null)
                return;

            await CurrentWord.AutoCompleteAsync((message) => _messageService.Publish(message));
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
                    MessageBus.Current.SendMessage(new WordCreatedMessage(CurrentWord.Id));

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