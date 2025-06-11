using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMaster.Data.Services.Interfaces;
using WordMaster.Data.ViewModels;
using WordMasterApp.Features;

namespace WordMasterApp.DIFactories
{
    public interface IWordDetailsViewModelDIFactory
    {
        WordDetailsViewModel Create(IObservable<WordWrapperViewModel?> word);
    }

    public class WordDetailsViewModelDIFactory : IWordDetailsViewModelDIFactory
    {
        private readonly IWordService _service;
        private readonly IWordUsageViewModelDIFactory _wordUsageFactory;

        public WordDetailsViewModelDIFactory(IWordService service, IWordUsageViewModelDIFactory wordUsageFactory)
        {
            _service = service;
            _wordUsageFactory = wordUsageFactory;
        }

        public WordDetailsViewModel Create(IObservable<WordWrapperViewModel?> word)
        {
            return new WordDetailsViewModel(word, _service, _wordUsageFactory);
        }
    }
}
