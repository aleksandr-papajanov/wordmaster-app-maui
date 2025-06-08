using WordMaster.Data.Infrastructure;

namespace WordMasterApp
{
    public partial class App : Application
    {
        public App(IServiceProvider provider)
        {
            InitializeComponent();

            Task.Run(async () =>
            {
                var seeder = provider.GetService<IDataSeeder>();

                if (seeder != null)
                {
                    await seeder.SeedIfNeededAsync();
                }
            });

            MainPage = new AppShell();
        }

    }
}
