using WordMasterApp.ViewModels.Word;

namespace WordMasterApp.Views;

public partial class WordListPage : BaseContentPage
{
	public WordListPage(WordListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {

    }
}