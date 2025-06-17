using WordMasterApp.DataLayer;

namespace WordMasterApp
{
    public partial class AppShell : Shell
    {
        private readonly RealmDataContext _context;

        public AppShell(RealmDataContext context)
        {
            _context = context;
            InitializeComponent();

            //Routing.RegisterRoute(nameof(WordDetails), typeof(WordDetailsPage));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _context.Open();
        }
    }
}
