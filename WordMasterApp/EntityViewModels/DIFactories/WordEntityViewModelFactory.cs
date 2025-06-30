using DynamicData;
using WordMaster.Data.Models;
using WordMaster.Data.Services.Interfaces;
using WordMaster.Generation.Interfaces;
using WordMasterApp.EntityViewModels.Actions;

namespace WordMasterApp.EntityViewModels.DIFactories
{
    public interface IWordEntityViewModelFactory
    {
        WordEntityViewModel Create(Word entity);
        WordEntityViewModel Create(DeckEntityViewModel parent);
    }

    public class WordEntityViewModelFactory : IWordEntityViewModelFactory
    {
        private readonly IWordService _wordService;
        private readonly IDeckService _deckService;
        private readonly IWordUsageService _wordUsageService;
        private readonly IRelatedWordService _relatedWordService;
        private readonly IGenerationService _generationService;
        private readonly Lazy<IDeckEntityViewModelFactory> _deckFactory;
        private readonly IWordUsageEntityViewModelFactory _wordUsageFactory;
        private readonly IRelatedWordEntityViewModelFactory _relatedWordFactory;

        public WordEntityViewModelFactory(
            IWordService wordService,
            IDeckService deckService,
            IWordUsageService wordUsageService,
            IRelatedWordService relatedWordService,
            IGenerationService generationService,
            Lazy<IDeckEntityViewModelFactory> deckFactory,
            IWordUsageEntityViewModelFactory wordUsageFactory,
            IRelatedWordEntityViewModelFactory relatedWordFactory)
        {
            _wordService = wordService;
            _deckService = deckService;
            _wordUsageService = wordUsageService;
            _relatedWordService = relatedWordService;
            _generationService = generationService;
            _deckFactory = deckFactory;
            _wordUsageFactory = wordUsageFactory;
            _relatedWordFactory = relatedWordFactory;
        }

        private WordEntityViewModelActions CreateActions(WordEntityViewModel viewModel, Word entity)
        {
            return new(
               () =>
               {
                   var deck = _deckService.Find(entity.Id)
                        ?? throw new Exception("Source language not found");

                   return _deckFactory.Value.Create(deck);
               },
               filter =>
               {
                   return _wordUsageService
                       .GetStream(entity.Id, filter)
                       .Transform(w => _wordUsageFactory.Create(w));
               },
               type =>
               {
                   return _relatedWordService
                       .GetStream(entity.Id, type)
                       .Transform(w => _relatedWordFactory.Create(w));
               },
               async () =>
               {
                   if (entity.IsManaged)
                   {
                       throw new Exception("Can not create entity that already exists");
                   }

                   await _wordService.CreateAsync(entity);
               },
               async () =>
               {
                   if (!entity.IsManaged)
                   {
                       throw new Exception("Can not delete detouched entity");
                   }

                   await _wordService.DeleteAsync(entity);
               },
               async updater =>
               {
                   if (!entity.IsManaged)
                   {
                       throw new Exception("Can not update detouched entity");
                   }

                   await _wordService.UpdateAsync(entity, updater);
               },
               request =>
               {
                   return _generationService.CompleteWordDetails(request);
               }
           );
        }

        public WordEntityViewModel Create(Word entity)
        {
            if (!entity.IsManaged)
            {
                throw new Exception("Can not create wrapper of detouched entity");
            }

            var viewModel = new WordEntityViewModel(entity);
            viewModel.Actions = CreateActions(viewModel, entity);

            return viewModel;
        }

        public WordEntityViewModel Create(DeckEntityViewModel parent)
        {
            if (!parent.IsManaged)
            {
                throw new Exception("Can not create wrapper of detouched entity");
            }

            var entity = new Word
            {
                DeckId = parent.Id
            };

            var viewModel = new WordEntityViewModel(entity);
            viewModel.Actions = CreateActions(viewModel, entity);

            return viewModel;
        }
    }
}
