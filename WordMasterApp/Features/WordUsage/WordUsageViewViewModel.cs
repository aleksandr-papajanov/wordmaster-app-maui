using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using WordMasterApp.DIFactories;
using WordMasterApp.EntityWrappers;
using WordMasterApp.Services;
using WordMasterApp.Services.Generation;

namespace WordMasterApp.Features
{
    public class WordUsageViewViewModel : ReactiveObject, IValidatableViewModel, IActivatableViewModel
    {
        private readonly IGenerationService _aiService;
        private readonly IWordUsageWrapperFactory _usageWrapperFactory;
        private readonly BehaviorSubject<string> _searchTextSubject = new(string.Empty);

        private readonly ObservableAsPropertyHelper<WordWrapper?> _currentWord;
        public WordWrapper? CurrentWord => _currentWord.Value;

        // Main collection
        private  ReadOnlyObservableCollection<WordUsageWrapper> _usages;
        public ReadOnlyObservableCollection<WordUsageWrapper> Usages => _usages;

        // Search text binded to the UI entry field
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        // Binded to SelectedItem of CollectionView
        private WordUsageWrapper? _selectedUsage;
        public WordUsageWrapper? SelectedUsage
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
        public ICommand GenerateUsageCommand { get; private set; }
        public ICommand CreateUsageCommand { get; private set; }
        public ICommand UpdateUsageCommand { get; private set; }
        public ICommand EditUsageCommand { get; private set; }
        public ICommand DeleteUsageCommand { get; private set; }
        public ICommand SelectUsageCommand { get; private set; }

        // Implementing IValidatableViewModel requires a ValidationContext property
        public IValidationContext ValidationContext { get; } = new ValidationContext();

        // Implementing IActivatableViewModel requires an Activator property
        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public WordUsageViewViewModel(IObservable<WordWrapper?> word,
                                      IWordUsageService usageService,
                                      IGenerationService aiService,
                                      IWordUsageWrapperFactory wordUsageWrapperFactory)
        {
            _aiService = aiService;
            _usageWrapperFactory = wordUsageWrapperFactory;

            word.ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.CurrentWord, out _currentWord);

            SetupValidation();
            SetupCommands();

            this.WhenActivated(disposables =>
            {
                Observable
                    .CombineLatest(word, _searchTextSubject.AsObservable(), (word, filter) => (word, filter))
                    .Select(x => x.word == null
                        ? Observable.Return(ChangeSet<WordUsageWrapper>.Empty)
                        : usageService.GetStream(x.word.Id, x.filter))
                    .Switch()
                    .Sort(SortExpressionComparer<WordUsageWrapper>.Descending(x => x.CreatedAt))
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
            var word    = this.WhenAnyValue(x => x.CurrentWord).Select(x => x != null);
            var managed = this.WhenAnyValue(x => x.CurrentWord).Select(x => x != null && x.IsManaged);
            var usage   = this.WhenAnyValue(x => x.SelectedUsage).Select(x => x != null);
            var valid   = this.WhenAnyValue(x => x.ValidationContext.IsValid);
            var view    = this.WhenAnyValue(x => x.Mode, mode => mode == WordUsageViewMode.View);
            var edit    = this.WhenAnyValue(x => x.Mode, mode => mode == WordUsageViewMode.Edit);
            var create  = this.WhenAnyValue(x => x.Mode, mode => mode == WordUsageViewMode.Create);

            var canGen    = Observable.CombineLatest(word, managed, usage, valid, view, edit, create, (word, managed, usage, valid, view, edit, create) => managed && view);
            var canCreate = Observable.CombineLatest(word, managed, usage, valid, view, edit, create, (word, managed, usage, valid, view, edit, create) => managed && (view || create));
            var canUpdate = Observable.CombineLatest(word, managed, usage, valid, view, edit, create, (word, managed, usage, valid, view, edit, create) => valid && ((usage && edit) || create));
            var canEdit   = Observable.CombineLatest(word, managed, usage, valid, view, edit, create, (word, managed, usage, valid, view, edit, create) => usage && (view || edit));
            var canDelete = Observable.CombineLatest(word, managed, usage, valid, view, edit, create, (word, managed, usage, valid, view, edit, create) => usage && view);

            GenerateUsageCommand = ReactiveCommand.CreateFromTask(GenerateUsageAsync, canGen);
            CreateUsageCommand   = ReactiveCommand.Create(Create, canCreate);
            UpdateUsageCommand   = ReactiveCommand.CreateFromTask(UpdateAsync, canUpdate);
            EditUsageCommand     = ReactiveCommand.Create(Edit, canEdit);
            DeleteUsageCommand   = ReactiveCommand.CreateFromTask(DeleteAsync, canDelete);
            SelectUsageCommand   = ReactiveCommand.Create<WordUsageWrapper>(OnUsageTapped);
        }



        private async Task GenerateUsageAsync()
        {
            if (CurrentWord == null)
                return;

            var request = new WordUsageGenerationRequest(
                word: CurrentWord.Text,
                translation: CurrentWord.Translation,
                sourceLang: "Swedish",
                targetLang: "Russian",
                count: 1
            );

        //    var response = await _aiService.GenerateExamples(request);

        //    foreach (var example in response.Examples)
        //    {
        //        var newUsage = _usageWrapperFactory.CreateNew(CurrentWord);

        //        newUsage.Text = example.Key;
        //        newUsage.Translation = example.Value;
                
        //        await newUsage.SaveAsync();
        //    }
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
                
                await SelectedUsage.SaveAsync();

                Text = string.Empty;
                Translation = string.Empty;
                Mode = WordUsageViewMode.View;
            }

            if (Mode == WordUsageViewMode.Create)
            {
                if (CurrentWord == null)
                    return;

                var newUsage = _usageWrapperFactory.CreateNew(CurrentWord);
                newUsage.Text = Text;
                newUsage.Translation = Translation;

                await newUsage.SaveAsync();
                
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

        private void OnUsageTapped(WordUsageWrapper tapped)
        {
            SelectedUsage = SelectedUsage != tapped
                ? tapped
                : null;
        }
    }
}
