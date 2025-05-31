using WordMasterApp.Views;

namespace WordMasterApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(WordListPage), typeof(WordListPage));
            Routing.RegisterRoute(nameof(WordPage), typeof(WordPage));
        }
    }
}
