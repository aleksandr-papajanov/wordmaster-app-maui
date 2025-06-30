using DynamicData;
using WordMaster.Data.Models;
using WordMaster.Data.Services.Interfaces;
using WordMasterApp.EntityViewModels.Actions;

namespace WordMasterApp.EntityViewModels.DIFactories
{
    public interface IDeckEntityViewModelFactory
    {
        DeckEntityViewModel Create(Deck entity);
    }

    public class DeckEntityViewModelFactory : IDeckEntityViewModelFactory
    {
        private readonly IDeckService _deckService;
        private readonly IWordService _wordService;
        private readonly ILanguageService _languageService;
        private readonly IWordEntityViewModelFactory _wordFactory;
        private readonly ILanguageEntityViewModelFactory _languageFactory;

        public DeckEntityViewModelFactory(IDeckService deckService, IWordService wordService, ILanguageService languageService, IWordEntityViewModelFactory wordFactory, ILanguageEntityViewModelFactory languageFactory)
        {
            _deckService = deckService;
            _wordService = wordService;
            _languageService = languageService;
            _wordFactory = wordFactory;
            _languageFactory = languageFactory;
        }


        private DeckEntityViewModelActions CreateActions(DeckEntityViewModel viewModel, Deck entity)
        {
            return new(
               () =>
               {
                   var lang = _languageService.Find(entity.SourceLanguageCode)
                        ?? throw new Exception("Source language not found");

                   return _languageFactory.Create(lang);
               },
               () =>
               {
                   var lang = _languageService.Find(entity.TargetLanguageCode)
                        ?? throw new Exception("Target language not found");

                   return _languageFactory.Create(lang);
               },
               filter =>
               {
                   return _wordService
                       .GetStream(entity.Id, filter)
                       .Transform(w => _wordFactory.Create(w));
               },
               async () =>
               {
                   if (entity.IsManaged)
                   {
                       throw new Exception("Can not create entity that already exists");
                   }

                   await _deckService.CreateAsync(entity);
               },
               async () =>
               {
                   if (!entity.IsManaged)
                   {
                       throw new Exception("Can not delete detouched entity");
                   }

                   await _deckService.DeleteAsync(entity);
               },
               async updater =>
               {
                   if (!entity.IsManaged)
                   {
                       throw new Exception("Can not update detouched entity");
                   }

                   await _deckService.UpdateAsync(entity, updater);
               }
           );
        }

        public DeckEntityViewModel Create(Deck entity)
        {
            if (!entity.IsManaged)
            {
                throw new Exception("Can not create wrapper of detouched entity");
            }

            var viewModel = new DeckEntityViewModel(entity);
            viewModel.Actions = CreateActions(viewModel, entity);

            return viewModel;
        }
    }
}
