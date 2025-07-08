using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using WordMaster.AIClientProvider;
using WordMaster.Data;
using WordMaster.Data.Services;
using WordMaster.Generation;
using WordMaster.Generation.Interfaces;
using WordMasterApp.EntityViewModels.DIFactories;
using WordMasterApp.Features;
using WordMasterApp.Features.DeckList;
using WordMasterApp.Features.MainPage;
using WordMasterApp.Features.MessageContainer;
using WordMasterApp.Features.MessageContainer.Interfaces;
using WordMasterApp.Features.RelatedWords;
using WordMasterApp.Features.WordDetails;
using WordMasterApp.Features.WordList;
using WordMasterApp.Features.WordUsage;

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

            // Wrapper factories
            builder.Services.AddSingleton<ILanguageEntityViewModelFactory, LanguageEntityViewModelFactory>();
            builder.Services.AddSingleton<IDeckEntityViewModelFactory, DeckEntityViewModelFactory>();
            builder.Services.AddSingleton<IWordEntityViewModelFactory, WordEntityViewModelFactory>();
            builder.Services.AddSingleton<IWordDetailsEntityViewModelFactory, WordDetailsEntityViewModelFactory>();
            builder.Services.AddSingleton<IWordUsageEntityViewModelFactory, WordUsageEntityViewModelFactory>();
            builder.Services.AddSingleton<IWordRelationEntityViewModelFactory, WordRelationEntityViewModelFactory>();

            // NOTE:
            // We explicitly use Lazy<T> here for factories that reference each other *up* the object graph,
            // to avoid circular dependency issues during service resolution.
            //
            // For example, DeckEntityViewModelFactory internally depends on WordEntityViewModelFactory,
            // and WordEntityViewModelFactory in turn refer back to DeckEntityViewModelFactory to lookup which deck it is in.
            //
            // This approach is intentionally applied only to keep the architecture maintainable
            // and avoid injecting IServiceProvider or using service location anti-patterns.
            builder.Services.AddSingleton(sp => new Lazy<IDeckEntityViewModelFactory>(() => sp.GetRequiredService<IDeckEntityViewModelFactory>()));
            builder.Services.AddSingleton(sp => new Lazy<IWordEntityViewModelFactory>(() => sp.GetRequiredService<IWordEntityViewModelFactory>()));

            // Data and services
            builder.Services.AddSingleton<RealmDataContext>();
            builder.Services.AddTransient(typeof(IRepository<>), typeof(RealmRepository<>));
            builder.Services.AddTransient<IDeckService, DeckService>();
            builder.Services.AddTransient<IWordService, WordService>();

            builder.Services.AddTransient<ISessionBuilder, SessionBuilder>();

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

            // ViewModel factories
            builder.Services.AddTransient<IDeckListViewViewModelFactory, DeckListViewViewModelFactory>();
            builder.Services.AddTransient<IWordListViewViewModelFactory, WordListViewViewModelFactory>();

            // Pages and ViewModels
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainViewModel>();

            builder.Services.AddTransient<WordDetailsView>();
            builder.Services.AddTransient<WordDetailsViewModel>();
            builder.Services.AddTransient<IWordDetailsViewViewModelFactory, WordDetailsViewViewModelFactory>();

            builder.Services.AddTransient<WordUsageView>();
            builder.Services.AddTransient<WordUsageViewViewModel>();
            builder.Services.AddTransient<IWordUsageViewViewModelFactory, WordUsageViewViewModelFactory>();
            
            builder.Services.AddTransient<RelatedWordsView>();
            builder.Services.AddTransient<RelatedWordsViewViewModel>();
            builder.Services.AddTransient<IRelatedWordsViewViewModelFactory, RelatedWordsViewViewModelFactory>();
            
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
                    fonts.AddFont("noto-sans-regular.ttf", "NotoSansRegular");
                    fonts.AddFont("noto-sans-bold.ttf", "NotoSansBold");
                    fonts.AddFont("noto-sans-light.ttf", "NotoSansLight");
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
