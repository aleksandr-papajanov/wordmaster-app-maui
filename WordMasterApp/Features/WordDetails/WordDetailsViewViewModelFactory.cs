using WordMasterApp.EntityViewModels;
using WordMasterApp.Features.MessageContainer;
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

        public WordDetailsViewViewModelFactory(
            IMessageService messageService,
            IWordUsageViewViewModelFactory wordUsageFactory)
        {
            _messageService = messageService;
            _usageFactory = wordUsageFactory;
        }

        public WordDetailsViewModel Create(IObservable<WordEntityViewModel?> word)
        {
            return new WordDetailsViewModel(word, _messageService, _usageFactory);
        }
    }
}
