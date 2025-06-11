using DynamicData;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using WordMaster.Data.Services.Interfaces;
using WordMasterApp.EntityWrappers;

namespace WordMasterApp.Features.DeckList
{
    public class DeckListViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly IDeckService _deckService;

        private ReadOnlyObservableCollection<DeckWrapperViewModel> _decks = null!;
        public ReadOnlyObservableCollection<DeckWrapperViewModel> Decks => _decks;

        private DeckWrapperViewModel? _selectedDeck;
        public DeckWrapperViewModel? SelectedDeck
        {
            get => _selectedDeck;
            set => this.RaiseAndSetIfChanged(ref _selectedDeck, value);
        }

        public ICommand SelectDeckCommand { get; }

        // Implementing IActivatableViewModel requires an Activator property
        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public DeckListViewModel(IDeckService deckService)
        {
            _deckService = deckService;

            SelectDeckCommand = ReactiveCommand.Create<DeckWrapperViewModel>(SelectDeck);

            this.WhenActivated(disposables =>
            {
                _deckService.GetChangeSet()
                    .Transform(x => new DeckWrapperViewModel(x, deckService))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out _decks)
                    .DisposeMany()
                    .Subscribe(_ =>
                    {
                        this.RaisePropertyChanged(nameof(Decks));
                    })
                    .DisposeWith(disposables);
            });
            
        }

        private void SelectDeck(DeckWrapperViewModel tapped)
            => SelectedDeck = SelectedDeck != tapped
                ? tapped 
                : null;
    }
}
