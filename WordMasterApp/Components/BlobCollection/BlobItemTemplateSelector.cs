using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMaster.Data.DTOs;
using WordMasterApp.ViewModels.Word;

namespace WordMasterApp.Components
{
    public class BlobItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate RegularTemplate { get; set; }
        public DataTemplate SelectedTemplate { get; set; }
        public DataTemplate CreateTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is BlobCollectionItem<IDisblayable> blob && blob.OriginalItem is NewWordPlaceholder)
            {
                return CreateTemplate;
            }

            return RegularTemplate;
        }
    }
}
