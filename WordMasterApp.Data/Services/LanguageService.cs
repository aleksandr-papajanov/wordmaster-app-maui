using WordMasterApp.Data.Models;
using WordMasterApp.Data.Services.Interfaces;

namespace WordMasterApp.Data.Services
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
