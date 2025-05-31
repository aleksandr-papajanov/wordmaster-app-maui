using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WordMaster.Data.DTOs;
using WordMaster.Data.Infrastructure.Interfaces;
using WordMasterApp.Infrastructure.Interfaces;
using WordMasterApp.Views;

namespace WordMasterApp.ViewModels.Word
{
    public partial class WordListViewModel : ObservableObject//, IAsyncInitialize
    {
        private readonly INavigationService _navigation;
        private readonly IWordService _wordService;

        [ObservableProperty]
        private ObservableCollection<WordDTO> _words = new();

        public WordListViewModel(INavigationService navigation, IWordService wordService)
        {
            _navigation = navigation;
            _wordService = wordService;

            _wordService.Words.Subscribe(words =>
            {
                Words.Clear();

                foreach (var word in words)
                {
                    Words.Add(word);
                }
            });
        }

        [RelayCommand]
        private async Task CreateWordAsync()
        {
            await _navigation.NavigateToAsync($"{nameof(WordPage)}?new=true");
        }
        

        [RelayCommand]
        private async Task SelectWordAsync(Guid id)
        {
            await _navigation.NavigateToAsync($"{nameof(WordPage)}?id={id}");
        }
    }
}
