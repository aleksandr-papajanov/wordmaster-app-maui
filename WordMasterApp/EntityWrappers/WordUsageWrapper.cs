using Microsoft.Maui.Controls;
using WordMasterApp.DataLayer.Models;
using WordMasterApp.Services;

namespace WordMasterApp.EntityWrappers
{
    public class WordUsageWrapper : EntityWrapperBase<WordUsage>
    {
        private readonly IWordUsageService _usageService;

        //Navigation property to the associated Word, if any
        private WordWrapper? _word;
        public WordWrapper? Word => _word ??= _usageService.GetWord(_entity);

        // ViewModel properties
        public Guid Id { get; private set; }
        public string Text { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt => _entity.CreatedAt;

        public WordUsageWrapper(WordWrapper word, IWordUsageService usageService) : base(new WordUsage())
        {
            if (!word.IsManaged)
            {
                throw new Exception("Can not create wrapper of detouched entity");
            }

            _usageService = usageService;

            _entity.WordId = word.Id;

            BindProperties();
        }

        public WordUsageWrapper(WordUsage entity, IWordUsageService usageService) : base(entity)
        {
            if (!entity.IsManaged)
            {
                throw new Exception("Can not create wrapper of detouched entity");
            }

            _usageService = usageService;

            Id = _entity.Id;
            Text = _entity.Text;
            Translation = _entity.Translation;

            BindProperties();
        }

        private void BindProperties()
        {
            _entity.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(WordUsage.Id))
                {
                    Id = _entity.Id;
                }
                else if (e.PropertyName == nameof(WordUsage.Text))
                {
                    Text = _entity.Text;
                }
                else if (e.PropertyName == nameof(WordUsage.Translation))
                {
                    Translation = _entity.Translation;
                }
            };
        }


        public async Task SaveAsync()
        {
            if (IsManaged)
            {
                await _usageService.UpdateAsync(_entity, (t) =>
                {
                    _entity.Text = Text;
                    _entity.Translation = Translation;
                });
            }
            else
            {
                _entity.Text = Text;
                _entity.Translation = Translation;

                await _usageService.CreateAsync(_entity);
            }
        }

        public async Task DeleteAsync()
        {
            await _usageService.DeleteAsync(_entity);
        }
    }
}


