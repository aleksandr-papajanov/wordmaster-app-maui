using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using Realms;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using WordMaster.Data.Models;
using WordMaster.Data.Services.Interfaces;
using WordMaster.Data.ViewModels;
using WordMasterApp.DIFactories;

namespace WordMasterApp.Features
{
    public class WordUsageViewViewModel : ReactiveObject, IValidatableViewModel, IActivatableViewModel
    {
        private readonly IWordUsageWrapperViewModelDIFactory _wordUsageWrapperFactory;
        private readonly BehaviorSubject<string> _searchTextSubject = new(string.Empty);


        // Main collection
        private ReadOnlyObservableCollection<WordUsageWrapperViewModel> _usages;
        public ReadOnlyObservableCollection<WordUsageWrapperViewModel> Usages => _usages;

        private WordWrapperViewModel _currentWord;
        public WordWrapperViewModel CurrentWord
        {
            get => _currentWord;
            set => this.RaiseAndSetIfChanged(ref _currentWord, value);
        }

        // Search text binded to the UI entry field
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        // Binded to SelectedItem of CollectionView
        private WordUsageWrapperViewModel? _selectedUsage;
        public WordUsageWrapperViewModel? SelectedUsage
        {
            get => _selectedUsage;
            set => this.RaiseAndSetIfChanged(ref _selectedUsage, value);
        }

        // Determines whether the edit form is visible in the UI
        private bool _isEditFormVisible;
        public bool IsEditFormVisible
        {
            get => _isEditFormVisible;
            set => this.RaiseAndSetIfChanged(ref _isEditFormVisible, value);
        }
        
        private WordUsageViewMode _mode = WordUsageViewMode.View;
        public WordUsageViewMode Mode
        {
            get => _mode;
            set => this.RaiseAndSetIfChanged(ref _mode, value);
        }

        // Edit form properties
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

        // Commands
        public ICommand CreateCommand { get; private set; }
        public ICommand UpdateCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand SelectCommand { get; private set; }


        public WordUsageViewViewModel(IObservable<WordWrapperViewModel?> word, IWordUsageService service, IWordUsageWrapperViewModelDIFactory wordUsageWrapperFactory)
        {
            _wordUsageWrapperFactory = wordUsageWrapperFactory;

            SetupValidation();
            SetupCommands();

            this.WhenActivated(disposables =>
            {
                Observable
                    .CombineLatest(word, _searchTextSubject.AsObservable(), (word, filter) => (word, filter))
                    .Select(x => x.word == null
                        ? Observable.Return(ChangeSet<WordUsage>.Empty)
                        : service.GetStream(x.word.Id, x.filter))
                    .Switch()
                    .Transform(x => wordUsageWrapperFactory.Create(x))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out _usages)
                    .DisposeMany()
                    .Subscribe(_ => this.RaisePropertyChanged(nameof(Usages)))
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.SearchText)
                    .Throttle(TimeSpan.FromMilliseconds(80))
                    .Select(text => text?.Trim() ?? string.Empty)
                    .DistinctUntilChanged()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(text => _searchTextSubject.OnNext(text))
                    .DisposeWith(disposables);

                word
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, x => x.CurrentWord)
                    .DisposeWith(disposables);

                word
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(_ =>
                    {
                        Mode = WordUsageViewMode.View;
                        SelectedUsage = null;
                        Text = string.Empty;
                        Translation = string.Empty;
                    })
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.Mode)
                    .Select(mode => mode == WordUsageViewMode.Edit || mode == WordUsageViewMode.Create)
                    .BindTo(this, x => x.IsEditFormVisible)
                    .DisposeWith(disposables);
            });
        }

        private void SetupValidation()
        {
            this.ValidationRule(
                vm => vm.Text,
                text => !string.IsNullOrWhiteSpace(text),
                "Usage example cannot be empty"
            );

            this.ValidationRule(
                vm => vm.Translation,
                text => !string.IsNullOrWhiteSpace(text),
                "Translation cannot be empty"
            );
        }

        private void SetupCommands()
        {
            var word   = this.WhenAnyValue(x => x.CurrentWord).Select(x => x != null);
            var usage  = this.WhenAnyValue(x => x.SelectedUsage).Select(x => x != null);
            var valid  = this.WhenAnyValue(x => x.ValidationContext.IsValid);
            var view   = this.WhenAnyValue(x => x.Mode, mode => mode == WordUsageViewMode.View);
            var edit   = this.WhenAnyValue(x => x.Mode, mode => mode == WordUsageViewMode.Edit);
            var create = this.WhenAnyValue(x => x.Mode, mode => mode == WordUsageViewMode.Create);

            var canCreate = Observable.CombineLatest(word, usage, valid, view, edit, create, (word, usage, valid, view, edit, create) => word && (view || create));
            var canUpdate = Observable.CombineLatest(word, usage, valid, view, edit, create, (word, usage, valid, view, edit, create) => valid && ((usage && edit) || create));
            var canEdit   = Observable.CombineLatest(word, usage, valid, view, edit, create, (word, usage, valid, view, edit, create) => usage && (view || edit));
            var canDelete = Observable.CombineLatest(word, usage, valid, view, edit, create, (word, usage, valid, view, edit, create) => usage && view);

            CreateCommand = ReactiveCommand.Create(Create, canCreate);
            UpdateCommand = ReactiveCommand.CreateFromTask(UpdateAsync, canUpdate);
            EditCommand   = ReactiveCommand.Create(Edit, canEdit);
            DeleteCommand = ReactiveCommand.CreateFromTask(DeleteAsync, canDelete);
            SelectCommand = ReactiveCommand.Create<WordUsageWrapperViewModel>(OnUsageTapped);
        }



        private void Create()
        {
            if (Mode == WordUsageViewMode.View)
            {
                Mode = WordUsageViewMode.Create;
                SelectedUsage = null;
                Text =
                Translation = string.Empty;
            }
            else
            {
                Mode = WordUsageViewMode.View;
            }
        }
        
        private void Edit()
        {
            if (SelectedUsage == null)
                return;

            if (Mode == WordUsageViewMode.View)
            {
                Mode = WordUsageViewMode.Edit;
                Text = SelectedUsage.Text;
                Translation = SelectedUsage.Translation;
            }
            else
            {
                Mode = WordUsageViewMode.View;
            }
        }

        private async Task UpdateAsync()
        {
            if (Mode == WordUsageViewMode.Edit)
            {
                if (SelectedUsage == null)
                    return;

                SelectedUsage.Text = Text;
                SelectedUsage.Translation = Translation;
                await SelectedUsage.UpdateAsync();

                Text = string.Empty;
                Translation = string.Empty;
                Mode = WordUsageViewMode.View;
            }

            if (Mode == WordUsageViewMode.Create)
            {
                if (CurrentWord == null)
                    return;

                var newUsage = _wordUsageWrapperFactory.Create(CurrentWord.Entity);
                newUsage.Text = Text;
                newUsage.Translation = Translation;

                await newUsage.UpdateAsync();
                
                // Reset the form
                Text = string.Empty;
                Translation = string.Empty;
                Mode = WordUsageViewMode.View;
            }
        }

        private async Task DeleteAsync()
        {
            if (SelectedUsage == null)
                return;

            await SelectedUsage.DeleteAsync();
        }

        private void OnUsageTapped(WordUsageWrapperViewModel tapped)
        {
            if (SelectedUsage == tapped)
            {
                SelectedUsage = null;
            }
            else
            {
                SelectedUsage = tapped;
            }
        }


        // Implementing IValidatableViewModel requires a ValidationContext property
        public IValidationContext ValidationContext { get; } = new ValidationContext();

        // Implementing IActivatableViewModel requires an Activator property
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
