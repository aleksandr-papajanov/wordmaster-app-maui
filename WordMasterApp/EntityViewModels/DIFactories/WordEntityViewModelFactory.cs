using DynamicData;
using WordMaster.Data.Models;
using WordMaster.Data.Services;
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
        private readonly ISessionBuilder _generator;
        private readonly Lazy<IDeckEntityViewModelFactory> _deckFactory;
        private readonly IWordDetailsEntityViewModelFactory _wordDetailsFactory;
        private readonly IWordUsageEntityViewModelFactory _wordUsageFactory;
        private readonly IWordRelationEntityViewModelFactory _relatedWordFactory;

        public WordEntityViewModelFactory(
            IWordService wordService,
            IDeckService deckService,
            ISessionBuilder generator,
            Lazy<IDeckEntityViewModelFactory> deckFactory,
            IWordDetailsEntityViewModelFactory wordDetailsFactory,
            IWordUsageEntityViewModelFactory wordUsageFactory,
            IWordRelationEntityViewModelFactory relatedWordFactory)
        {
            _wordService = wordService;
            _deckService = deckService;
            _generator = generator;
            _deckFactory = deckFactory;
            _wordDetailsFactory = wordDetailsFactory;
            _wordUsageFactory = wordUsageFactory;
            _relatedWordFactory = relatedWordFactory;
        }

        private WordEntityViewModelActions CreateActions(WordEntityViewModel viewModel, Word entity, DeckEntityViewModel? createdWith = null)
        {
            return new WordEntityViewModelActions
            {
                GetDetails = () => entity.Details != null
                    ? _wordDetailsFactory.Create(entity.Details)
                    : null,
                GetParentDeck = () =>
                {
                    if (entity.IsManaged)
                    {
                        var deck = _deckService.FindByWord(entity)
                            ?? throw new Exception("Can not find parent deck for word entity");

                        return _deckFactory.Value.Create(deck);
                    }
                    else
                    {
                        if (createdWith == null)
                        {
                            throw new ArgumentNullException(nameof(createdWith), "Deck must be provided for new word creation");
                        }

                        return createdWith;
                    }
                },
                GetUsagesStream = filter =>
                {
                    return _wordService
                        .GetUsagesStream(entity, filter)
                        .Transform(w => _wordUsageFactory.Create(w));
                },
                GetRelatedWordsStream = type =>
                {
                    return _wordService
                        .GetRelationsStream(entity, type)
                        .Transform(w => _relatedWordFactory.Create(w));
                },
                CreateDetails = (pronunciation, partOfSpeech, partOfSpeechType) => _wordService.CreateDetailsAsync(entity, pronunciation, partOfSpeech, partOfSpeechType),
                CreateRelation = (word, relationType) => _wordService.CreateRelationAsync(entity, word, relationType),
                CreateUsage = (sentence, translation) => _wordService.CreateUsageAsync(entity, sentence, translation),
                CreateAsync = async () =>
                {
                    if (createdWith == null)
                    {
                        throw new ArgumentNullException(nameof(createdWith), "Deck must be provided for new word creation");
                    }

                    await _wordService.CreateWordAsync(entity, createdWith.Entity);
                },
                DeleteAsync = async (DeckEntityViewModel deck) =>
                {
                    if (deck == null)
                    {
                        throw new ArgumentNullException(nameof(deck), "Deck must be provided for new word creation");
                    }

                    await _wordService.DeleteWordFromDeckAsync(entity, deck.Entity);
                },
                UpdateAsync = async updater =>
                {
                    await _wordService.UpdateWordAsync(entity, updater);
                }
            };
        }

        public WordEntityViewModel Create(Word entity)
        {
            if (!entity.IsManaged)
            {
                throw new Exception("Can not create wrapper of detouched entity");
            }

            var viewModel = new WordEntityViewModel(entity, _generator);
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
                Id = Guid.NewGuid(),
                Text = string.Empty
            };

            var viewModel = new WordEntityViewModel(entity, _generator);
            viewModel.Actions = CreateActions(viewModel, entity, parent);

            return viewModel;
        }
    }
}
