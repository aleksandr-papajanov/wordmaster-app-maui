using DynamicData;
using System.Reactive.Subjects;
using WordMasterApp.Components.BlobCollection;

namespace WordMasterApp.Infrastructure
{
    // Renamed the class to avoid conflict with the existing definition
    public interface IBlobCollectionViewModelFactory
    {
        BlobCollectionViewModel Create(ISubject<IObservable<IChangeSet<IBlobCollectionDisplayable>>> stream);
    }

    public class BlobCollectionViewModelFactory : IBlobCollectionViewModelFactory
    {
        public BlobCollectionViewModel Create(ISubject<IObservable<IChangeSet<IBlobCollectionDisplayable>>> stream) =>
            new BlobCollectionViewModel(stream);
    }
}
