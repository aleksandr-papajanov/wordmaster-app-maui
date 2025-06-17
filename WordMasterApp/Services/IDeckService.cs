using DynamicData;
using WordMasterApp.EntityWrappers;

namespace WordMasterApp.Services
{
    public interface IDeckService
    {
        IObservable<IChangeSet<DeckWrapper>> GetStream();

        DeckWrapper? Find(Guid id);

    }
}