using WordMaster.Data.DTOs;
using WordMaster.Data.Models;

namespace WordMaster.Data.Infrastructure.Interfaces
{
    public interface IWordService
    {
        public IObservable<IList<WordDTO>> Words { get; }
        public WordDTO? Find(Guid id);
        public Task<WordDTO> CreateAsync(WordDTO dto);
        public Task DeleteAsync(Guid id);
        public Task UpdateAsync(WordDTO dto);
    }
}