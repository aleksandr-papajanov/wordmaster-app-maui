using DynamicData;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using WordMasterApp.EntityViewModels;

namespace WordMasterApp.Features.DeckList
{
    public class DeckListViewModel : ReactiveObject, IActivatableViewModel
    {
        private ReadOnlyObservableCollection<DeckEntityViewModel> _decks = null!;
        public ReadOnlyObservableCollection<DeckEntityViewModel> Decks => _decks;

        private DeckEntityViewModel? _selectedDeck;
        public DeckEntityViewModel? SelectedDeck
        {
            get => _selectedDeck;
            set => this.RaiseAndSetIfChanged(ref _selectedDeck, value);
        }

        public ICommand SelectDeckCommand { get; }

        // Implementing IActivatableViewModel requires an Activator property
        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public DeckListViewModel(IObservable<IChangeSet<DeckEntityViewModel>> deckStream)
        {
            SelectDeckCommand = ReactiveCommand.Create<DeckEntityViewModel>(SelectDeck);

            this.WhenActivated(disposables =>
            {
                deckStream
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

        private void SelectDeck(DeckEntityViewModel tapped)
            => SelectedDeck = SelectedDeck != tapped
                ? tapped 
                : null;
    }
}
