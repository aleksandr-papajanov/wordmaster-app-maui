using WordMaster.Data.Models;

namespace WordMaster.Data.Infrastructure
{
    public class DataSeeder : IDataSeeder
    {
        private readonly IRepository<AppSettings> appSettingsRep;
        private readonly IRepository<Language> languageRep;
        private readonly IRepository<Deck> deckRep;
        private readonly IRepository<Word> wordRep;
        private readonly IRepository<WordUsage> wordUsageRep;

        public DataSeeder(IRepository<AppSettings> appSettingsRep,
                          IRepository<Language> languageRep,
                          IRepository<Deck> deckRep,
                          IRepository<Word> wordRep,
                          IRepository<WordUsage> wordUsageRep)
        {
            this.appSettingsRep = appSettingsRep;
            this.languageRep = languageRep;
            this.deckRep = deckRep;
            this.wordRep = wordRep;
            this.wordUsageRep = wordUsageRep;
        }

        public async Task SeedIfNeededAsync()
        {
            var settings = appSettingsRep.Find("global");

            if (settings == null || !settings.IsInitialized)
            {
                await SeedAsync();
            }
        }

        private async Task SeedAsync()
        {

            var id = new GuidGenerator();

            var data = new
            {
                languages = new[]
                {
                    new { code = "sw", name = "svenska" },
                    new { code = "ru", name = "русский" }
                },
                decks = new[]
                {
                    new {
                        id = id.Next(),
                        name = "Default Deck",
                        source_lang = "sw",
                        target_lang = "ru",
                        words = new[]
                        {
                            new {
                                id = id.Next(),
                                text = "åtkomast",
                                translation = "доступный",
                                definition = "som kan nås eller användas",
                                usages = new[] {
                                    new { text = "Den här filen är inte åtkomast för alla.", translation = "Этот файл недоступен для всех." },
                                    new { text = "Gör dokumentet åtkomast för användare.", translation = "Сделайте документ доступным для пользователей." }
                                }
                            },
                            new {
                                id = id.Next(),
                                text = "givetvis",
                                translation = "разумеется",
                                definition = "självklart, naturligtvis",
                                usages = new[] {
                                    new { text = "Givetvis kommer jag att hjälpa dig.", translation = "Разумеется, я помогу тебе." },
                                    new { text = "Det är givetvis viktigt.", translation = "Это, разумеется, важно." }
                                }
                            },
                            new {
                                id = id.Next(),
                                text = "plågsam",
                                translation = "мучительный",
                                definition = "som orsakar smärta eller obehag",
                                usages = new[] {
                                    new { text = "Det var en plågsam upplevelse.", translation = "Это был мучительный опыт." },
                                    new { text = "Väntan blev plågsam.", translation = "Ожидание стало мучительным." }
                                }
                            },
                            new {
                                id = id.Next(),
                                text = "fångar",
                                translation = "пленники / ловит",
                                definition = "personer som hålls fångna; att ta fast något",
                                usages = new[] {
                                    new { text = "Fångar rymde från fängelset.", translation = "Пленники сбежали из тюрьмы." },
                                    new { text = "Han fångar bollen.", translation = "Он ловит мяч." }
                                }
                            },
                            new {
                                id = id.Next(),
                                text = "förebygger",
                                translation = "предотвращает",
                                definition = "att förhindra att något händer",
                                usages = new[] {
                                    new { text = "Vaccin förebygger sjukdomar.", translation = "Вакцина предотвращает болезни." },
                                    new { text = "God kost förebygger hälsoproblem.", translation = "Правильное питание предотвращает проблемы со здоровьем." }
                                }
                            },
                            new {
                                id = id.Next(),
                                text = "gnäller",
                                translation = "ныть",
                                definition = "klagar på ett irriterande sätt",
                                usages = new[] {
                                    new { text = "Barnet gnäller hela tiden.", translation = "Ребёнок всё время ноет." },
                                    new { text = "Sluta gnälla och gör något åt det.", translation = "Перестань ныть и сделай что-нибудь." }
                                }
                            },
                            new {
                                id = id.Next(),
                                text = "bökig",
                                translation = "неуклюжий",
                                definition = "klumpig, svår att hantera",
                                usages = new[] {
                                    new { text = "Det var en bökig process.", translation = "Это был неуклюжий процесс." },
                                    new { text = "Han är lite bökig ibland.", translation = "Он иногда неуклюжий." }
                                }
                            },
                            new {
                                id = id.Next(),
                                text = "fordon",
                                translation = "транспортное средство",
                                definition = "något man färdas med",
                                usages = new[] {
                                    new { text = "Det finns många olika fordon på vägen.", translation = "На дороге много разных транспортных средств." },
                                    new { text = "Vilket fordon kör du?", translation = "На каком транспортном средстве ты ездишь?" }
                                }
                            },
                            new {
                                id = id.Next(),
                                text = "smidig",
                                translation = "гибкий / ловкий",
                                definition = "lätt att röra sig; enkel att använda",
                                usages = new[] {
                                    new { text = "Hon är smidig som en katt.", translation = "Она гибкая, как кошка." },
                                    new { text = "Systemet är smidigt att använda.", translation = "Системой удобно пользоваться." }
                                }
                            },
                            new {
                                id = id.Next(),
                                text = "särskilja",
                                translation = "различать",
                                definition = "se skillnad på",
                                usages = new[] {
                                    new { text = "Det är svårt att särskilja dem.", translation = "Трудно их различить." },
                                    new { text = "Kan du särskilja färgerna?", translation = "Ты можешь различать цвета?" }
                                }
                            },
                            new {
                                id = id.Next(),
                                text = "överstiger",
                                translation = "превышает",
                                definition = "är mer än",
                                usages = new[] {
                                    new { text = "Priset överstiger min budget.", translation = "Цена превышает мой бюджет." },
                                    new { text = "Temperaturen överstiger 30 grader.", translation = "Температура превышает 30 градусов." }
                                }
                            },
                            new {
                                id = id.Next(),
                                text = "överlag",
                                translation = "в целом",
                                definition = "i allmänhet, generellt",
                                usages = new[] {
                                    new { text = "Överlag var resultatet bra.", translation = "В целом результат был хорошим." },
                                    new { text = "Överlag är jag nöjd.", translation = "В целом я доволен." }
                                }
                            },
                            new {
                                id = id.Next(),
                                text = "preliminär",
                                translation = "предварительный",
                                definition = "inte slutgiltig, tillfällig",
                                usages = new[] {
                                    new { text = "Detta är en preliminär rapport.", translation = "Это предварительный отчёт." },
                                    new { text = "Tiden är preliminär.", translation = "Время предварительное." }
                                }
                            },
                            new {
                                id = id.Next(),
                                text = "efterhand",
                                translation = "постепенно / со временем",
                                definition = "med tiden, så småningom",
                                usages = new[] {
                                    new { text = "Vi lär oss mer efterhand.", translation = "Со временем мы узнаём больше." },
                                    new { text = "Efterhand blev det lättare.", translation = "Постепенно стало легче." }
                                }
                            },
                            new {
                                id = id.Next(),
                                text = "fördom",
                                translation = "предрассудок",
                                definition = "en förutfattad mening",
                                usages = new[] {
                                    new { text = "Han har en fördom mot nya idéer.", translation = "У него предрассудок против новых идей." },
                                    new { text = "Fördomar kan vara skadliga.", translation = "Предрассудки могут быть вредными." }
                                }
                            },
                            new {
                                id = id.Next(),
                                text = "genomsnitt",
                                translation = "среднее",
                                definition = "medelvärde",
                                usages = new[] {
                                    new { text = "Genomsnittet är högt.", translation = "Среднее значение высокое." },
                                    new { text = "Vad är genomsnittet?", translation = "Какое среднее значение?" }
                                }
                            }
                        }
                    }
                }
            }; // seed

            using (var trans = await appSettingsRep.BeginWriteAsync())
            {
                appSettingsRep.Create(new AppSettings()
                {
                    Id = "global",
                    IsInitialized = true,
                });

                await trans.CommitAsync();
            }

            using (var trans = await languageRep.BeginWriteAsync())
            {
                foreach (var item in data.languages)
                {
                    languageRep.Create(new Language
                    {
                        Code = item.code,
                        Name = item.name
                    });
                }

                await trans.CommitAsync();
            }
            
            using (var trans = await deckRep.BeginWriteAsync())
            {
                foreach (var deck in data.decks)
                {
                    deckRep.Create(new Deck
                    { 
                        Id = deck.id,
                        Name = deck.name,
                        SourceLanguageCode = deck.source_lang,
                        TargetLanguageCode = deck.target_lang
                    });

                    foreach (var word in deck.words)
                    {
                        wordRep.Create(new Word
                        {
                            Id = word.id,
                            DeckId = deck.id,
                            Text = word.text,
                            Translation = word.translation,
                            Definition = word.definition,
                            CreatedAt = DateTimeOffset.UtcNow,
                            ModifiedAt = DateTimeOffset.UtcNow,
                        });


                        foreach (var usage in word.usages)
                        {
                            wordUsageRep.Create(new WordUsage
                            {
                                Id = Guid.NewGuid(),
                                WordId = word.id,
                                Text = usage.text,
                                Translation = usage.translation,
                                CreatedAt = DateTimeOffset.UtcNow,
                                ModifiedAt = DateTimeOffset.UtcNow,
                            });
                        }
                    }
                }

                await trans.CommitAsync();
            }
           
        }
    }


    public class GuidGenerator
    {
        private readonly Dictionary<string, Guid> ids = new();

        public Guid Last { get; private set; }

        public Guid Next(string? key = null)
        {
            if (key != null)
            {
                if (!ids.TryGetValue(key, out var found))
                {
                    var newId = Guid.NewGuid();
                    ids.Add(key, newId);
                    return newId;
                }
                else
                {
                    return found;
                }
            }
            else
            {
                return Last = Guid.NewGuid();
            }
        }
    }
}