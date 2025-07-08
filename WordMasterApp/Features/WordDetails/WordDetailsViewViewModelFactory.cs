using WordMasterApp.EntityViewModels;
using WordMasterApp.Features.MessageContainer.Interfaces;
using WordMasterApp.Features.RelatedWords;
using WordMasterApp.Features.WordUsage;

namespace WordMasterApp.Features.WordDetails
{
    public interface IWordDetailsViewViewModelFactory
    {
        WordDetailsViewModel Create(IObservable<WordEntityViewModel?> word);
    }

    public class WordDetailsViewViewModelFactory : IWordDetailsViewViewModelFactory
    {
        private readonly IMessageService _messageService;
        private readonly IWordUsageViewViewModelFactory _usageFactory;
        private readonly IRelatedWordsViewViewModelFactory _relatedWordsFactory;

        public WordDetailsViewViewModelFactory(
            IMessageService messageService,
            IWordUsageViewViewModelFactory wordUsageFactory,
            IRelatedWordsViewViewModelFactory relatedWordsFactory)
        {
            _messageService = messageService;
            _usageFactory = wordUsageFactory;
            _relatedWordsFactory = relatedWordsFactory;
        }

        public WordDetailsViewModel Create(IObservable<WordEntityViewModel?> word)
        {
            return new WordDetailsViewModel(word, _messageService, _usageFactory, _relatedWordsFactory);
        }
    }
}
