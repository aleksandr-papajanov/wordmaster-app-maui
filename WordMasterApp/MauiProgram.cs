using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using WordMaster.Data.Infrastructure;
using WordMaster.Data.Services;
using WordMasterApp.Features;
using WordMasterApp.Infrastructure;

namespace WordMasterApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.Services.AddTransient<IDataContext, RealmDataContext>();
            builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddSingleton<IWordService, WordService>();
            builder.Services.AddSingleton<IWordUsageService, WordUsageService>();

            builder.Services.AddTransient<IWordUsageViewModelFactory, WordUsageViewModelFactory>();
            builder.Services.AddTransient<IWordViewModelFactory, WordViewModelFactory>();
            builder.Services.AddTransient<IBlobCollectionViewModelFactory, BlobCollectionViewModelFactory>();

            builder.Services.AddTransient<WordDetailsPage>();
            builder.Services.AddTransient<WordDetailsViewModel>();

            builder.Services.AddTransient<WordUsageView>();
            builder.Services.AddTransient<WordUsageViewModel>();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
