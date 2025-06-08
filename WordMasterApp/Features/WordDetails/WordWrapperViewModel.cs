using ReactiveUI;
using WordMaster.Data.Models;
using WordMaster.Data.Services;
using WordMasterApp.Components.BlobCollection;

namespace WordMaster.Data.ViewModels
{
    public class WordWrapperViewModel : ReactiveObject, IBlobCollectionDisplayable
    {
        private readonly IWordService _service;
        private readonly Word _entity;

        public bool IsManaged => _entity.IsManaged;
        public Word Entity => _entity;


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

        public WordWrapperViewModel(IWordService wordService) : this(new Word(), wordService)
        {
        }

        public WordWrapperViewModel(Word entity, IWordService service)
        {
            _entity = entity;
            _service = service;

            _id = _entity.Id;
            _text = _entity.Text;
            _translation = _entity.Translation;
            _definition = _entity.Definition;

            _entity.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Word.Id))
                {
                    Id = _entity.Id;
                }
                if (e.PropertyName == nameof(Word.Text))
                {
                    Text = _entity.Text;
                }
                else if (e.PropertyName == nameof(Word.Translation))
                {
                    Translation = _entity.Translation;
                }
                else if (e.PropertyName == nameof(Word.Definition))
                {
                    Definition = _entity.Definition;
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
                    entity.Definition = Definition;
                });
            }
            else
            {
                _entity.Id = Id;
                _entity.Text = Text;
                _entity.Translation = Translation;
                _entity.Definition = Definition;

                await _service.CreateAsync(_entity);
            }
        }

        public async Task DeleteAsync()
        {
            await _service.DeleteAsync(_entity);
        }
    }
}
