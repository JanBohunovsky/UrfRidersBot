using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.Util;
using UrfRiders.Modules.AutoVoice;
using UrfRiders.Modules.Clash;
using UrfRiders.Modules.Covid19;
using UrfRiders.Modules.Interactive;
using UrfRiders.Services;

namespace UrfRiders
{
    class Program
    {
        public const string Version = "1.4";
        public const uint Color = 0x05b3eb;

        //static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private static async Task Main(string[] args)
        {
            await using var services = ConfigureServices();
            var client = services.GetRequiredService<DiscordSocketClient>();
            var config = services.GetRequiredService<IConfiguration>();

            // Initialize logic for services
            services.GetRequiredService<LogService>();
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
            services.GetRequiredService<InteractiveService>();
            services.GetRequiredService<AutoVoiceService>();
            //services.GetRequiredService<Covid19Service>();
            services.GetRequiredService<ClashService>();

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
                .AddLogging(config => config.AddConsole().AddFile("bot.log").SetMinimumLevel(LogLevel.Information))
#endif
                .AddSingleton<LogService>()
                // Extra
                .AddSingleton(BuildConfig())
                .AddSingleton(BuildRiotApiConfig)
                .AddSingleton(new LiteDatabase("bot.db"))
                .AddSingleton<HttpClient>()
                .AddSingleton<CommandHelper>()
                .AddSingleton(services => RiotApi.NewInstance(services.GetRequiredService<IRiotApiConfig>()))
                // My Services
                .AddSingleton<InteractiveService>()
                .AddSingleton<AutoVoiceService>()
                //.AddSingleton<Covid19Service>()
                .AddSingleton<ClashService>()
                .BuildServiceProvider();
        }

        private static IConfiguration BuildConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();
        }

        private static IRiotApiConfig BuildRiotApiConfig(IServiceProvider services)
        {
            var config = services.GetRequiredService<IConfiguration>();
            return new RiotApiConfig.Builder(config["riot_api_key"])
            {

            }.Build();
        }
    }
}
