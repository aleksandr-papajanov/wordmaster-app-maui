using ReactiveUI;

namespace WordMasterApp.Components.BlobCollection
{
    public partial class BlobCollectionItem<T> : ReactiveObject
        where T : IBlobCollectionDisplayable
    {
        public Guid Id { get; }
        
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

        public T OriginalItem { get; }


        public BlobCollectionItem(T item)
        {
            Id = item.Id;
            OriginalItem = item;
            Text = item.Text;
        }
    }
}
