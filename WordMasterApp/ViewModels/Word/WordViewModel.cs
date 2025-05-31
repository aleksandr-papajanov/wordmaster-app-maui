using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WordMaster.Data.DTOs;
using WordMaster.Data.Infrastructure.Interfaces;
using WordMaster.Data.Models;
using WordMasterApp.Components;

using WordMasterApp.Infrastructure.Interfaces;

namespace WordMasterApp.ViewModels.Word
{
    public partial class WordViewModel : ObservableObject, IQueryAttributable
    {
        private readonly INavigationService _navigation;
        private readonly IWordService _wordService;
        private ObservableCollection<IDisblayable> _words = new();
        private IDisblayable? _selectedWord;

        public ObservableCollection<IDisblayable> Words
        {
            get => _words;
            set => SetProperty(ref _words, value);
        }

        public IDisblayable? SelectedWord
        {
            get => _selectedWord;
            set
            {
                SetProperty(ref _selectedWord, value);

                UpdateCommand.NotifyCanExecuteChanged();
                DeleteCommand.NotifyCanExecuteChanged();
            }
        }

        private bool CanUpdate => SelectedWord is WordDTO;
        private bool CanDelete => SelectedWord != null && SelectedWord is not NewWordPlaceholder;


        public WordViewModel(INavigationService navigation, IWordService wordService)
        {
            _navigation = navigation;
            _wordService = wordService;

            _wordService.Words.Subscribe(words =>
            {
                Words.Clear();
                Words.Add(new NewWordPlaceholder());

                foreach (var word in words)
                {
                    Words.Add(word);
                }
            });
        }


        [RelayCommand(CanExecute = nameof(CanUpdate))]
        private async Task UpdateAsync()
        {
            if (SelectedWord is NewWordPlaceholder placeholder)
            {
                var created = await _wordService.CreateAsync(placeholder);
                SelectedWord = created;
            }
            else if (SelectedWord is WordDTO word)
            {
                await _wordService.UpdateAsync(word);
            }
        }

        [RelayCommand(CanExecute = nameof(CanDelete))]
        private async Task DeleteAsync()
        {
            if (SelectedWord is WordDTO word && word.Id != Guid.Empty)
            {
                await _wordService.DeleteAsync(word.Id);

                SelectedWord = null;
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("id", out var raw) && Guid.TryParse(raw?.ToString(), out var id))
            {
                var word = _words.FirstOrDefault(w => w.Id == id);

                if (word != null)
                {
                    SelectedWord = word;
                }
            }
        }
    }
}

