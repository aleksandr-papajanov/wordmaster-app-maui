using WordMasterApp.DataLayer;

namespace WordMasterApp
{
    public partial class App : Application
    {
        public App(RealmDataContext context)
        {
            InitializeComponent();

            MainPage = new AppShell(context);
        }
    }
}
