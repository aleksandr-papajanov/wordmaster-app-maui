using DynamicData;
using System.Reactive.Subjects;
using WordMasterApp.Components.BlobCollection;

namespace WordMasterApp.DIFactories
{
    public interface IBlobCollectionViewModelDIFactory
    {
        BlobCollectionViewModel Create(ISubject<IObservable<IChangeSet<IBlobCollectionDisplayable>>> stream);
    }

    public class BlobCollectionViewModelDIFactory : IBlobCollectionViewModelDIFactory
    {
        public BlobCollectionViewModel Create(ISubject<IObservable<IChangeSet<IBlobCollectionDisplayable>>> stream) =>
            new BlobCollectionViewModel(stream);
    }
}
