using WordMasterApp.DataLayer.Models;
using WordMasterApp.Services;

namespace WordMasterApp.EntityWrappers
{
    public class DeckWrapper : EntityWrapperBase<Deck>
    {
        private readonly IDeckService _service;
        
        public Guid Id { get; private set; }
        public string Name { get; set; } = string.Empty;

        public DeckWrapper(Deck entity, IDeckService service) : base(entity)
        {
            if (!entity.IsManaged)
            {
                throw new Exception("Can not create wrapper of detouched entity");
            }

            _service = service;

            Id = _entity.Id;
            Name = _entity.Name;

            BindProperties();
        }

        private void BindProperties()
        {
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
    }
}
