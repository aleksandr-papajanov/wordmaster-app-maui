using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using WordMaster.Data.Infrastructure;
using WordMaster.Data.Services;
using WordMaster.Data.Services.Interfaces;
using WordMasterApp.DIFactories;
using WordMasterApp.Features;
using WordMasterApp.Features.MainPage;
using WordMasterApp.Features.WordDetails;

namespace WordMasterApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // di factories
            builder.Services.AddTransient<IDeckListViewModelDIFactory, DeckListViewModelDIFactory>(); // DeckListView

            builder.Services.AddTransient<IWordListViewModelDIFactory, WordListViewModelDIFactory>(); // WordListView
            builder.Services.AddTransient<IWordDetailsViewModelDIFactory, WordDetailsViewModelDIFactory>(); // WordDetailsView
            builder.Services.AddTransient<IWordUsageViewModelDIFactory, WordUsageViewModelDIFactory>(); // WordUsageView
            
            builder.Services.AddTransient<IWordUsageWrapperViewModelDIFactory, WordUsageWrapperViewModelDIFactory>(); // wrapper for WordUsage
            builder.Services.AddTransient<IWordWrapperViewModelDIFactory, WordWrapperViewModelDIFactory>(); // wrapper for Word
            

            // data and services
            builder.Services.AddTransient<IDataContext, RealmDataContext>();
            builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddTransient<IDataSeeder, DataSeeder>();
            builder.Services.AddSingleton<IDeckService, DeckService>();
            builder.Services.AddSingleton<IWordService, WordService>();
            builder.Services.AddSingleton<IWordUsageService, WordUsageService>();

            //components

            // pages and viewmodels
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainViewModel>();

            builder.Services.AddTransient<WordDetailsView>();
            builder.Services.AddTransient<WordDetailsViewModel>();

            builder.Services.AddTransient<WordUsageView>();
            builder.Services.AddTransient<WordUsageViewViewModel>();

            

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("fa-solid-900.ttf", "FontAwesomeSolid");
                    fonts.AddFont("fa-regular-400.ttf", "FontAwesomeRegular");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif


            return builder.Build();
        }
    }
}
