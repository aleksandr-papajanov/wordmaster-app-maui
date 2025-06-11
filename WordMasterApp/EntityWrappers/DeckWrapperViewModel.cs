using ReactiveUI;
using WordMaster.Data.Models;
using WordMaster.Data.Services.Interfaces;
using WordMasterApp.Components.BlobCollection;

namespace WordMasterApp.EntityWrappers
{
    public class DeckWrapperViewModel : EntityWrapperBase<Deck>
    {
        private readonly IDeckService _service;
        
        private Guid _id;
        public Guid Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }


        //public DeckWrapperViewModel(IDeckService wordService) : this(new Word(), wordService)
        //{
        //}

        public DeckWrapperViewModel(Deck entity, IDeckService service) : base(entity)
        {
            _service = service;

            _id = _entity.Id;
            _name = _entity.Name;

            _entity.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Deck.Id))
                {
                    Id = _entity.Id;
                }
                else if (e.PropertyName == nameof(Deck.Name))
                {
                    Name = _entity.Name;
                }
            };
        }


        //public async Task UpdateAsync()
        //{
        //    if (IsManaged)
        //    {
        //        await _service.UpdateAsync(_entity, (entity) =>
        //        {
        //            entity.Id = Id;
        //            entity.Text = Text;
        //            entity.Translation = Translation;
        //            entity.Definition = Definition;
        //        });
        //    }
        //    else
        //    {
        //        _entity.Id = Id;
        //        _entity.Text = Text;
        //        _entity.Translation = Translation;
        //        _entity.Definition = Definition;

        //        await _service.CreateAsync(_entity);
        //    }
        //}

        //public async Task DeleteAsync()
        //{
        //    await _service.DeleteAsync(_entity);
        //}
    }
}
