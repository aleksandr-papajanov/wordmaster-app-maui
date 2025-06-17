using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using ReactiveUI;
using WordMaster.AIClientProvider;
using WordMasterApp.DataLayer;
using WordMasterApp.DIFactories;
using WordMasterApp.Features;
using WordMasterApp.Features.MainPage;
using WordMasterApp.Features.MessageContainer;
using WordMasterApp.Features.WordDetails;
using WordMasterApp.Services;
using WordMasterApp.Services.Generation;

namespace WordMasterApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            builder.Configuration.AddConfiguration(configuration);

            // Регистрируем конфигурацию как сервис
            builder.Services.AddSingleton<IConfiguration>(configuration);

            // di factories
            builder.Services.AddTransient<IDeckListViewModelFactory, DeckListViewModelFactory>(); // DeckListView
            builder.Services.AddTransient<IWordListViewModelFactory, WordListViewModelFactory>(); // WordListView
            builder.Services.AddTransient<IWordDetailsViewModelFactory, WordDetailsViewModelFactory>(); // WordDetailsView
            builder.Services.AddTransient<IWordUsageViewModelFactory, WordUsageViewModelFactory>(); // WordUsageView

            builder.Services.AddTransient<IDeckWrapperFactory, DeckWrapperFactory>(); // wrapper for Deck
            builder.Services.AddTransient<IWordWrapperFactory, WordWrapperFactory>(); // wrapper for Word
            builder.Services.AddTransient<IWordUsageWrapperFactory, WordUsageWrapperFactory>(); // wrapper for WordUsage

            // data and services
            builder.Services.AddSingleton<RealmDataContext>();
            builder.Services.AddTransient(typeof(IRepository<>), typeof(RealmRepository<>));
            //builder.Services.AddTransient<IDataSeeder, DataSeeder>();

            builder.Services.AddSingleton<IDeckService, DeckService>();
            builder.Services.AddTransient(provider => new Lazy<IDeckService>(() => provider.GetRequiredService<IDeckService>()));

            builder.Services.AddSingleton<IWordService, WordService>();
            builder.Services.AddTransient(provider => new Lazy<IWordService>(() => provider.GetRequiredService<IWordService>()));

            builder.Services.AddSingleton<IWordUsageService, WordUsageService>();
            builder.Services.AddTransient(provider => new Lazy<IWordUsageService>(() => provider.GetRequiredService<IWordUsageService>()));

            builder.Services.AddSingleton<IGenerationService, GenerationService>();

            // AI client provider
            builder.Services.AddSingleton<IAIClientProvider, AIClientProvider>();
            builder.Services.AddSingleton<ChatClient>(x =>
            {
                var provider = x.GetRequiredService<IAIClientProvider>();
                var config = x.GetRequiredService<IConfiguration>();

                var client = provider.CreateOpenAIClient();
                var model = config["AppSettings:OpenAI:Model"] ?? throw new Exception("OpenAI model is not configured in appsettings.json");

                return client.GetChatClient(model);
            });

            // pages and viewmodels
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainViewModel>();

            builder.Services.AddTransient<WordDetailsView>();
            builder.Services.AddTransient<WordDetailsViewModel>();

            builder.Services.AddTransient<WordUsageView>();
            builder.Services.AddTransient<WordUsageViewViewModel>();
            
            builder.Services.AddTransient<MessageContainer>();
            builder.Services.AddTransient<MessageContainerViewModel>();

            builder.Services.AddSingleton<IMessageService, MessageService>();


            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit(options =>
                {
                    //options.SetShouldEnableSnackbarOnWindows(true);
                })
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
