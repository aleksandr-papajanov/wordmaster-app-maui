using WordMaster.Data.Models;

namespace WordMaster.Data.Services.Interfaces
{
    public interface ILanguageService
    {
        Language? Find(string code);
    }
}
