using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMaster.Generation.Interfaces;
using WordMasterApp.EntityViewModels;
using WordMasterApp.Features.MessageContainer.Interfaces;
using WordMasterApp.Features.WordUsage;

namespace WordMasterApp.Features.RelatedWords
{
    public interface IRelatedWordsViewViewModelFactory
    {
        RelatedWordsViewViewModel Create(IObservable<WordEntityViewModel?> word);
    }

    public class RelatedWordsViewViewModelFactory : IRelatedWordsViewViewModelFactory
    {
        private readonly IMessageService _messageService;
        private readonly ISessionBuilder _generatore;

        public RelatedWordsViewViewModelFactory(
            IMessageService messageService,
            ISessionBuilder generator)
        {
            _messageService = messageService;
            _generatore = generator;
        }

        public RelatedWordsViewViewModel Create(IObservable<WordEntityViewModel?> word)
        {
            return new RelatedWordsViewViewModel(word, _messageService, _generatore);
        }
    }
}
