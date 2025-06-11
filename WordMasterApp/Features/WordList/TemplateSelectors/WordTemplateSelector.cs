using Microsoft.Maui.Controls;
using WordMaster.Data.ViewModels;

namespace WordMasterApp.Features.WordList
{
    public class WordTemplateSelector : DataTemplateSelector
    {
        public DataTemplate RegularTemplate { get; set; }
        public DataTemplate NewWordTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            // Check if the item is a new object
            if (item is WordWrapperViewModel word && !word.IsManaged)
            {
                return NewWordTemplate;
            }

            // Default to the regular template
            return RegularTemplate;
        }
    }
}