using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using System.Data;
using WordMaster.Data.Infrastructure;
using WordMaster.Data.Services;
using WordMasterApp.Components;
using WordMasterApp.DIFactories;
using WordMasterApp.Features;

namespace WordMasterApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // di factories
            builder.Services.AddTransient<IWordUsageViewViewModelDIFactory, WordUsageViewViewModelDIFactory>(); // WordUsageView
            builder.Services.AddTransient<IWordUsageWrapperViewModelDIFactory, WordUsageWrapperViewModelDIFactory>(); // wrapper for WordUsage
            builder.Services.AddTransient<IWordWrapperViewModelDIFactory, WordWrapperViewModelDIFactory>(); // wrapper for Word
            builder.Services.AddTransient<IBlobCollectionViewModelDIFactory, BlobCollectionViewModelDIFactory>();

            // data and services
            builder.Services.AddTransient<IDataContext, RealmDataContext>();
            builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddTransient<IDataSeeder, DataSeeder>();
            builder.Services.AddSingleton<IWordService, WordService>();
            builder.Services.AddSingleton<IWordUsageService, WordUsageService>();

            //components

            // pages and viewmodels
            builder.Services.AddTransient<WordDetailsPage>();
            builder.Services.AddTransient<WordDetailsPageViewModel>();

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
