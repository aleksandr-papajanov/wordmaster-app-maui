using DynamicData;
using WordMaster.Data.Models;
using WordMaster.Data.Services;
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
        private readonly IWordEntityViewModelFactory _wordFactory;
        private readonly ILanguageEntityViewModelFactory _languageFactory;

        public DeckEntityViewModelFactory(IDeckService deckService, IWordService wordService, IWordEntityViewModelFactory wordFactory, ILanguageEntityViewModelFactory languageFactory)
        {
            _deckService = deckService;
            _wordService = wordService;
            _wordFactory = wordFactory;
            _languageFactory = languageFactory;
        }


        private DeckEntityViewModelActions CreateActions(DeckEntityViewModel viewModel, Deck entity)
        {
            return new DeckEntityViewModelActions
            {
                GetSourceLanguage = () => _languageFactory.Create(entity.SourceLanguage),
                GetTargetLanguage = () => _languageFactory.Create(entity.TargetLanguage),
                GetWordsStream = filter =>
                {
                    return _wordService
                        .GetWordsStream(entity, filter)
                        .Transform(w => _wordFactory.Create(w));
                }
            };
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
