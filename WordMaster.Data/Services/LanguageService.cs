using WordMaster.Data;
using WordMaster.Data.Models;
using WordMaster.Data.Services.Interfaces;

namespace WordMaster.Data.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly IRepository<Language> _repository;

        public LanguageService(IRepository<Language> repository)
        {
            _repository = repository;
        }

        public Language? Find(string code)
        {
            return _repository.Find(code);
        }
    }
}
