using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using WordMaster.Data.Models;
using WordMaster.Data.Services.Interfaces;
using WordMaster.Data.ViewModels;
using WordMasterApp.DIFactories;
using WordMasterApp.Messages;

namespace WordMasterApp.Features
{
    public partial class WordDetailsViewModel : ReactiveObject, IActivatableViewModel, IValidatableViewModel
    {
        private readonly ObservableAsPropertyHelper<WordWrapperViewModel?> _currentWord;
        public WordWrapperViewModel? CurrentWord => _currentWord.Value;

        private bool _hasTriedToUpdate = false;
        public bool HasTriedToUpdate
        {
            get => _hasTriedToUpdate;
            private set => this.RaiseAndSetIfChanged(ref _hasTriedToUpdate, value);
        }

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

        private string _definition = string.Empty;
        public string Definition
        {
            get => _definition;
            set => this.RaiseAndSetIfChanged(ref _definition, value);
        }

        // ViewModels
        public WordUsageViewViewModel WordUsageViewModel { get; }

        // Commands
        public ICommand UpdateCommand { get; private set; }

        // Implementing IValidatableViewModel
        public IValidationContext ValidationContext { get; } = new ValidationContext();

        // Implementing IActivatableViewModel
        public ViewModelActivator Activator { get; } = new ViewModelActivator();


        public WordDetailsViewModel(IObservable<WordWrapperViewModel?> word,
                                    IWordService wordService,
                                    IWordUsageViewModelDIFactory wordUsageFactory)
        {
            word.ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.CurrentWord, out _currentWord);

            SetupValidation();
            SetupCommands();

            // important to create the view models in constructor for propper binding
            WordUsageViewModel = wordUsageFactory.Create(word);

            this.WhenActivated(disposables =>
            {
                word.Subscribe(word =>
                {
                    if (word == null)
                    {
                        Text =
                        Translation =
                        Definition = string.Empty;
                    }
                    else
                    {
                        Text = word.Text;
                        Translation = word.Translation;
                        Definition = word.Definition;
                    }

                    HasTriedToUpdate = false;
                })
                .DisposeWith(disposables);
            });
        }

        private void SetupValidation()
        {
            this.ValidationRule(
                vm => vm.Text,
                text => !string.IsNullOrWhiteSpace(text),
                "Word cannot be empty"
            );

            this.ValidationRule(
                vm => vm.Translation,
                text => !string.IsNullOrWhiteSpace(text),
                "Translation cannot be empty"
            );
        }

        private void SetupCommands()
        {
            var word  = this.WhenAnyValue(x => x.CurrentWord).Select(x => x != null);
            var tried = this.WhenAnyValue(x => x.HasTriedToUpdate);
            var valid = ValidationContext.WhenAnyValue(x => x.IsValid);

            var canUpdate = Observable.CombineLatest(word, tried, valid, (word, tried, valid) => word && (tried ? valid : true));

            UpdateCommand = ReactiveCommand.CreateFromTask(UpdateWordAsync, canUpdate);
        }


        private async Task UpdateWordAsync()
        {
            if (CurrentWord == null)
                return;

            if (ValidationContext.IsValid)
            {
                CurrentWord.Text = Text;
                CurrentWord.Translation = Translation;
                CurrentWord.Definition = Definition;

                await CurrentWord.UpdateAsync();

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