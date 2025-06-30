using WordMasterApp.Data.Models;

namespace WordMasterApp.Data.Services.Interfaces
{
    public interface ILanguageService
    {
        Language? Find(string code);
    }
}
