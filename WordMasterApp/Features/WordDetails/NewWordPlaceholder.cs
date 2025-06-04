using WordMasterApp.Components.BlobCollection;

namespace WordMasterApp.Features
{
    public class NewWordPlaceholder : IBlobCollectionDisplayable
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Text { get; } = "New Word";
    }
}

