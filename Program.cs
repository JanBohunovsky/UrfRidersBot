using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UrfRiders.Services;

namespace UrfRiders
{
    class Program
    {
        public const string Version = "1.3.1";
        public const uint Color = 0x05b3eb;

        //static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private static async Task Main(string[] args)
        {
            await using var services = ConfigureServices();
            var client = services.GetRequiredService<DiscordSocketClient>();
            var config = services.GetRequiredService<IConfiguration>();

            //client.Ready += async () =>
            //{
            //};

            // Initialize logic for services
            services.GetRequiredService<LogService>();
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
            services.GetRequiredService<InteractiveService>();
            services.GetRequiredService<AutoVoiceService>();
            services.GetRequiredService<Covid19Service>();

            // Start the bot
            await client.LoginAsync(TokenType.Bot, config["token"]);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                // Base
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                // Logging
#if DEBUG
                .AddLogging(config => config.AddConsole().AddFile("bot.log").SetMinimumLevel(LogLevel.Debug))
#else
                .AddLogging(config => config.AddConsole().AddFile("bot.log").SetMinimumLevel(LogLevel.Debug))
#endif
                .AddSingleton<LogService>()
                // Extra
                .AddSingleton(BuildConfig())
                .AddSingleton(new LiteDatabase("bot.db"))
                .AddSingleton<HttpClient>()
                .AddSingleton<CommandHelper>()
                // My Services
                .AddSingleton<InteractiveService>()
                .AddSingleton<AutoVoiceService>()
                .AddSingleton<Covid19Service>()
                .BuildServiceProvider();
        }

        private static IConfiguration BuildConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();
        }
    }
}
