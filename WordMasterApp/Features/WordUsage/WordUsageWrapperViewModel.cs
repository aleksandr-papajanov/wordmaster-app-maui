using ReactiveUI;
using WordMaster.Data.Models;
using WordMaster.Data.Services;

namespace WordMaster.Data.ViewModels
{
    public class WordUsageWrapperViewModel : ReactiveObject
    {
        private readonly IWordUsageService _service;
        private readonly WordUsage _entity;
        private readonly Word? _word;

        public bool IsManaged => _entity.IsManaged;

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

        public WordUsageWrapperViewModel(Word word, IWordUsageService wordService) : this(new WordUsage(), wordService)
        {
            _word = word;
        }

        public WordUsageWrapperViewModel(WordUsage entity, IWordUsageService service)
        {
            _entity = entity;
            _service = service;

            _id = _entity.Id;
            _text = _entity.Text;
            _translation = _entity.Translation;

            _entity.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(WordUsage.Id))
                {
                    Id = _entity.Id;
                }
                if (e.PropertyName == nameof(WordUsage.Text))
                {
                    Text = _entity.Text;
                }
                else if (e.PropertyName == nameof(WordUsage.Translation))
                {
                    Translation = _entity.Translation;
                }
            };
        }


        public async Task UpdateAsync()
        {
            if (IsManaged)
            {
                await _service.UpdateAsync(_entity, (entity) =>
                {
                    entity.Id = Id;
                    entity.Text = Text;
                    entity.Translation = Translation;
                });
            }
            else
            {
                if (_word == null)
                {
                    throw new InvalidOperationException("Word must be set for unmanaged WordUsage.");
                }

                _entity.Id = Id;
                _entity.WordId = _word.Id;
                _entity.Text = Text;
                _entity.Translation = Translation;

                await _service.CreateAsync(_entity);
            }
        }

        public async Task DeleteAsync()
        {
            await _service.DeleteAsync(_entity);
        }
    }
}


