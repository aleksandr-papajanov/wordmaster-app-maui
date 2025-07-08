using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WordMasterApp.Components.LoadingStripe;
using WordMasterApp.EntityViewModels;
using WordMasterApp.Features.DeckList;
using WordMasterApp.Features.MessageContainer;
using WordMasterApp.Features.MessageContainer.Interfaces;
using WordMasterApp.Features.MessageContainer.Messages;
using WordMasterApp.Features.WordDetails;
using WordMasterApp.Features.WordList;
using WordMasterApp.Messages;

namespace WordMasterApp.Features.MainPage
{
    public class MainViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly IDeckListViewViewModelFactory _deckListFactory;
        private readonly IWordListViewViewModelFactory _wordListFactory;
        private readonly IWordDetailsViewViewModelFactory _wordDetailsFactory;
        public readonly IMessageService _messageService;

        private readonly Subject<DeckEntityViewModel?> _deckSubject = new();
        private readonly Subject<WordEntityViewModel?> _wordSubject = new();

        // Screen sections
        private ReactiveObject? _sidebarSection;
        public ReactiveObject? SidebarSection
        {
            get => _sidebarSection;
            set => this.RaiseAndSetIfChanged(ref _sidebarSection, value);
        }
        
        private ReactiveObject? _mainSection;
        public ReactiveObject? MainSection
        {
            get => _mainSection;
            set => this.RaiseAndSetIfChanged(ref _mainSection, value);
        }
        
        private ReactiveObject? _detailsSection;
        public ReactiveObject? DetailsSection
        {
            get => _detailsSection;
            set => this.RaiseAndSetIfChanged(ref _detailsSection, value);
        }
        
        private bool _isLoadingStripeAnimating;
        public bool IsLoadingStripeAnimating
        {
            get => _isLoadingStripeAnimating;
            set => this.RaiseAndSetIfChanged(ref _isLoadingStripeAnimating, value);
        }
        
        private LoadingStripeType _loadingStripeType;
        public LoadingStripeType LoadingStripeType
        {
            get => _loadingStripeType;
            set => this.RaiseAndSetIfChanged(ref _loadingStripeType, value);
        }

        // ViewModels
        private WordListViewViewModel? _wordList;
        public WordListViewViewModel? WordList
        {
            get => _wordList;
            set => this.RaiseAndSetIfChanged(ref _wordList, value);
        }
        
        private WordDetailsViewModel? _wordDetails;
        public WordDetailsViewModel? WordDetails
        {
            get => _wordDetails;
            set => this.RaiseAndSetIfChanged(ref _wordDetails, value);
        }

        public MessageContainerViewModel MessageContainer { get; }

        // Implementing IActivatableViewModel requires an Activator property
        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public MainViewModel(IDeckListViewViewModelFactory deckListFactory,
                             IWordListViewViewModelFactory wordListFactory,
                             IWordDetailsViewViewModelFactory wordDetailsFactory,
                             IMessageService messageService)
        {
            _deckListFactory = deckListFactory;
            _wordListFactory = wordListFactory;
            _wordDetailsFactory = wordDetailsFactory;
            _messageService = messageService;

            MessageContainer = new MessageContainerViewModel(_messageService);

            var deckListViewModel = deckListFactory.Create();
            SidebarSection = deckListViewModel;


            // Initialize the main sections with empty observables
            WordList = _wordListFactory.Create(_deckSubject.AsObservable());
            MainSection = WordList;

            WordDetails = _wordDetailsFactory.Create(_wordSubject.AsObservable());
            DetailsSection = WordDetails;


            this.WhenActivated(disposables =>
            {
                deckListViewModel
                    .WhenAnyValue(x => x.SelectedDeck)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(d => _deckSubject.OnNext(d))
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.WordList)
                    .WhereNotNull()
                    .Select(w => w.WhenAnyValue(x => x.SelectedWord))
                    .Switch()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(w => _wordSubject.OnNext(w))
                    .DisposeWith(disposables);

                _deckSubject.
                    Subscribe(deck =>
                    {
                        if (deck == null)
                        {
                            //MainSection = null;
                            //DetailsSection = null;
                        }
                        else 
                        {
                            if (WordList == null)
                            {
                                WordList = _wordListFactory.Create(_deckSubject.AsObservable().StartWith(deck));
                            }

                            MainSection = WordList;
                        }
                    });

                _wordSubject.
                    Subscribe(word =>
                    {
                        if (word == null)
                        {
                            //MainSection = null;
                            //DetailsSection = null;
                        }
                        else
                        {
                            if (WordDetails == null)
                            {
                                WordDetails = _wordDetailsFactory.Create(_wordSubject.AsObservable().StartWith(word));
                            }

                            DetailsSection = WordDetails;
                        }
                    });

                MessageBus.Current.Listen<LoadingStripeMessage>()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(message =>
                    {
                        IsLoadingStripeAnimating = message.IsAnimating;
                        LoadingStripeType = message.Type;
                    })
                    .DisposeWith(disposables);
            });
            
            
        }

        int i = 0;
        ErrorMessage box;
        public async Task Test()
        {
            if (i == 0)
            {
                box = new ErrorMessage("This is an error message!");
                _messageService.Publish(box);
                var res = await box.Completion;
            }
            else if (i == 1)
            {
                //var res = await box.Completion;
            }
            else if (i == 2)
            {
                //var res = await box.Completion;
            }
            else if (i == 3)
            {
                //box.Close();
            }

            i = i++ > 3 ? 0 : i;
        }
    }
}