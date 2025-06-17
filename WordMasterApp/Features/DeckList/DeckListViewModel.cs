using DynamicData;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using WordMasterApp.EntityWrappers;
using WordMasterApp.Services;

namespace WordMasterApp.Features.DeckList
{
    public class DeckListViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly IDeckService _deckService;

        private ReadOnlyObservableCollection<DeckWrapper> _decks = null!;
        public ReadOnlyObservableCollection<DeckWrapper> Decks => _decks;

        private DeckWrapper? _selectedDeck;
        public DeckWrapper? SelectedDeck
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

            SelectDeckCommand = ReactiveCommand.Create<DeckWrapper>(SelectDeck);

            this.WhenActivated(disposables =>
            {
                _deckService
                    .GetStream()
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

        private void SelectDeck(DeckWrapper tapped)
            => SelectedDeck = SelectedDeck != tapped
                ? tapped 
                : null;
    }
}
