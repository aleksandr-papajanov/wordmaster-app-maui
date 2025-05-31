using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Layouts;
using Microsoft.Extensions.Logging;
using WordMaster.Data.Infrastructure;
using WordMaster.Data.Infrastructure.Interfaces;
using WordMaster.Data.Services;
using WordMasterApp.Infrastructure.Interfaces;
using WordMasterApp.Services;
using WordMasterApp.ViewModels.Word;
using WordMasterApp.Views;

namespace WordMasterApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.Services.AddTransient<IDataContext, RealmDataContext>();
            builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddTransient<IWordService, WordService>();
            
            builder.Services.AddSingleton<INavigationService, NavigationService>();

            builder.Services.AddTransient<WordListViewModel>();
            builder.Services.AddTransient<WordListPage>();

            builder.Services.AddTransient<WordViewModel>();
            builder.Services.AddTransient<WordPage>();
            
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
