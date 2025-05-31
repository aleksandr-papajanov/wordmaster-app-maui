using System.Threading.Tasks;
using WordMasterApp.Converters;
using WordMasterApp.ViewModels.Word;

namespace WordMasterApp.Views;

public partial class WordPage : BaseContentPage
{
	public WordPage(WordViewModel viewModel)
	{
        InitializeComponent();

        // important to add converters before setting the BindingContext becouse the XAML parser will look for them
        Resources.Add("SelectedItemToColorMultiConverter", new SelectedItemToColorMultiConverter());

        BindingContext = viewModel;
    }

    private async void Back_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}