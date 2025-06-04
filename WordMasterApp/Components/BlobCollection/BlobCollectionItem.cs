using ReactiveUI;

namespace WordMasterApp.Components.BlobCollection
{
    public partial class BlobCollectionItem<T> : ReactiveObject
        where T : IBlobCollectionDisplayable
    {
        private string _text = string.Empty;
        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }

        public Guid Id { get; }
        public T OriginalItem { get; }

        public BlobCollectionItem(T item)
        {
            OriginalItem = item;
            Id = item.Id;
            Text = item.Text;
        }
    }
}
