using ReactiveUI;
using WordMaster.Data.Models;
using WordMaster.Data.Services;
using WordMasterApp.Components.BlobCollection;

namespace WordMaster.Data.ViewModels
{
    public class WordViewModel : ReactiveObject, IBlobCollectionDisplayable
    {
        private readonly IWordService _wordService;
        private readonly Word _word;

        public bool IsManaged => _word.IsManaged;


        private Guid _id;
        public Guid Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        private string _text;
        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }
        
        private string _translation;
        public string Translation
        {
            get => _translation;
            set => this.RaiseAndSetIfChanged(ref _translation, value);
        }
        
        private string _definition;
        public string Definition
        {
            get => _definition;
            set => this.RaiseAndSetIfChanged(ref _definition, value);
        }

        public WordViewModel(IWordService wordService) : this(new Word(), wordService)
        {
        }

        public WordViewModel(Word word, IWordService wordService)
        {
            _word = word;
            _wordService = wordService;

            _id = word.Id;
            _text = word.Text;
            _translation = word.Translation;
            _definition = word.Definition;

            _word.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Word.Id))
                {
                    Id = _word.Id;
                }
                if (e.PropertyName == nameof(Word.Text))
                {
                    Text = _word.Text;
                }
                else if (e.PropertyName == nameof(Word.Translation))
                {
                    Translation = _word.Translation;
                }
                else if (e.PropertyName == nameof(Word.Definition))
                {
                    Definition = _word.Definition;
                }
            };
        }

        
        public async Task UpdateAsync()
        {
            if (IsManaged)
            {
                await _wordService.UpdateAsync(_word, (word) =>
                {
                    word.Id = Id;
                    word.Text = Text;
                    word.Translation = Translation;
                    word.Definition = Definition;
                });
            }
            else
            {
                _word.Id = Id;
                _word.Text = Text;
                _word.Translation = Translation;
                _word.Definition = Definition;

                await _wordService.CreateAsync(_word);
            }
        }

        public async Task DeleteAsync()
        {
            await _wordService.DeleteAsync(_word);
        }
    }
}
