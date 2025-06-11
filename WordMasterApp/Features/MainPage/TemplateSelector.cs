using WordMasterApp.Features.DeckList;
using WordMasterApp.Features.WordDetails;
using WordMasterApp.Features.WordList;

namespace WordMasterApp.Features.MainPage
{
    public class TemplateSelector : ContentView
    {
        public static readonly BindableProperty ItemProperty =
            BindableProperty.Create(nameof(Item), typeof(object), typeof(TemplateSelector),
                propertyChanged: (bindable, _, newValue) =>
                {
                    if (bindable is not TemplateSelector selector)
                        return;

                    selector.Content = newValue switch
                    {
                        DeckListViewModel dl => new DeckListView(dl),
                        WordListViewViewModel wl => new WordListView(wl),
                        WordDetailsViewModel wd => new WordDetailsView(wd),
                        _ => null
                    };
                });

        public object Item
        {
            get => GetValue(ItemProperty);
            set => SetValue(ItemProperty, value);
        }
    }
}
