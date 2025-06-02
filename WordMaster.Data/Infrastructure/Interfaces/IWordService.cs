using DynamicData;
using Realms;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using WordMaster.Data.DTOs;
using WordMaster.Data.Models;

namespace WordMaster.Data.Infrastructure.Interfaces
{
    public interface IWordService
    {
        SourceList<WordDTO> WordsSource { get; }
        BehaviorSubject<string> FilterSubject { get; }

        WordDTO? Find(Guid id);
        Task<WordDTO> CreateAsync(WordDTO dto);
        Task DeleteAsync(Guid id);
        Task UpdateAsync(WordDTO dto);
    }
}