using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WordMaster.Data.ViewModels;
using WordMasterApp.DIFactories;
using WordMasterApp.EntityWrappers;
using WordMasterApp.Features.WordList;

namespace WordMasterApp.Features.MainPage
{
    public class MainViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly IDeckListViewModelDIFactory _deckListFactory;
        private readonly IWordListViewModelDIFactory _wordListFactory;
        private readonly IWordDetailsViewModelDIFactory _wordDetailsFactory;

        private readonly Subject<DeckWrapperViewModel?> _deckSubject = new();
        private readonly Subject<WordWrapperViewModel?> _wordSubject = new();

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

        // Implementing IActivatableViewModel requires an Activator property
        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public MainViewModel(IDeckListViewModelDIFactory deckListFactory,
                             IWordListViewModelDIFactory wordListFactory,
                             IWordDetailsViewModelDIFactory wordDetailsFactory)
        {
            _deckListFactory = deckListFactory;
            _wordListFactory = wordListFactory;
            _wordDetailsFactory = wordDetailsFactory;

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
                    .Subscribe(deck => _deckSubject.OnNext(deck))
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.WordList)
                    .WhereNotNull()
                    .Select(wordList => wordList.WhenAnyValue(x => x.SelectedWord))
                    .Switch()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(word => _wordSubject.OnNext(word))
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
            });
            
            
        }
    }
}