using DSharpPlus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Core.Configuration;
using DiscordConfiguration = UrfRidersBot.Core.Configuration.DiscordConfiguration;

namespace UrfRidersBot.Infrastructure
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

        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            // TODO: Automate this via reflection
            services.AddSingleton(configuration.GetSection(DiscordConfiguration.SectionName).Get<DiscordConfiguration>());
            services.AddSingleton(configuration.GetSection(RiotGamesConfiguration.SectionName).Get<RiotGamesConfiguration>());

            return services;
        }

        public static IServiceCollection AddDiscordBot(this IServiceCollection services)
        {
            // Project services
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