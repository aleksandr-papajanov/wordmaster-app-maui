using WordMasterApp.Features;

namespace WordMasterApp.Components.BlobCollection
{
    public class BlobItemTemplateSelector : DataTemplateSelector
    {
        public required DataTemplate RegularTemplate { get; set; }
        public required DataTemplate CreateTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is BlobCollectionItem<IBlobCollectionDisplayable> blob && blob.OriginalItem is NewWordPlaceholder)
            {
                return CreateTemplate;
            }

            return RegularTemplate;
        }
    }
}
