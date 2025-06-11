using WordMaster.Data.Models;
using WordMaster.Data.Services.Interfaces;
using WordMaster.Data.ViewModels;
using WordMasterApp.Features;

namespace WordMasterApp.DIFactories
{
    public interface IWordWrapperViewModelDIFactory
    {
        WordWrapperViewModel Create(Word entity);
        WordWrapperViewModel Create(Deck deck);
    }

    public class WordWrapperViewModelDIFactory : IWordWrapperViewModelDIFactory
    {
        private readonly IWordService _service;

        public WordWrapperViewModelDIFactory(IWordService service)
        {
            _service = service;
        }

        public WordWrapperViewModel Create(Word entity)
        {
            if (!entity.IsManaged)
            {
                throw new InvalidOperationException("Cannot create a WordWrapperViewModel for an unmanaged Word entity.");
            }

            return new WordWrapperViewModel(entity, _service);
        }
        
        public WordWrapperViewModel Create(Deck entity)
        {
            if (!entity.IsManaged)
            {
                throw new InvalidOperationException("Cannot create a WordWrapperViewModel for an unmanaged Deck entity.");
            }

            return new WordWrapperViewModel(entity, _service);
        }
    }
}
