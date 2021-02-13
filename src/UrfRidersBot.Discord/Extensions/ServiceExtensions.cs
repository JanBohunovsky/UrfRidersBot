using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Discord.Configuration;
using UrfRidersBot.Discord.Interactive;
using DiscordConfiguration = UrfRidersBot.Discord.Configuration.DiscordConfiguration;

namespace UrfRidersBot.Discord
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddFetchableHostedService<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService, IHostedService
        {
            return services
                .AddSingleton<TService, TImplementation>()
                .AddHostedService(provider => (TImplementation)provider.GetRequiredService<TService>());
        }

        public static IServiceCollection AddDiscordBot(this IServiceCollection services)
        {
            // Configuration
            services
                .AddSingleton<DiscordConfiguration>()
                // .AddOnReadyService<EmoteConfiguration>()
                .AddSingleton<RiotGamesConfiguration>();
            
            // Project services
            services
                .AddSingleton<IEmbedService, EmbedService>();

            services
                .AddFetchableHostedService<IInteractiveService, InteractiveService>()
                .AddFetchableHostedService<IAutoVoiceService, AutoVoiceService>();
            
            
            // Discord client and service
            services.AddSingleton(provider =>
            {
                var discordConfig = provider.GetRequiredService<DiscordConfiguration>();
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                
                var client = new DiscordClient(new DSharpPlus.DiscordConfiguration
                {
                    Intents = DiscordIntents.All,
                    TokenType = TokenType.Bot,
                    Token = discordConfig.Token,
                    LoggerFactory = loggerFactory,
                    AlwaysCacheMembers = true,
                });

                return client;
            });
            services.AddHostedService<DiscordService>();

            return services;
        }
    }
}