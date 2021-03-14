using DSharpPlus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Core.Configuration;
using UrfRidersBot.Core.Interfaces;
using UrfRidersBot.Infrastructure.HostedServices;
using DiscordConfiguration = UrfRidersBot.Core.Configuration.DiscordConfiguration;

namespace UrfRidersBot.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            // TODO: Automate this via reflection
            services.AddSingleton(configuration.GetSection(DiscordConfiguration.SectionName).Get<DiscordConfiguration>());
            services.AddSingleton(configuration.GetSection(RiotGamesConfiguration.SectionName).Get<RiotGamesConfiguration>());

            return services;
        }

        public static IServiceCollection AddDiscordBot(this IServiceCollection services)
        {
            // TODO: Automate this via reflection
            // Project services
            services
                .AddHostedService<AutoVoiceHostedService>()
                .AddHostedService<ReactionRolesHostedService>();
            
            services.AddSingleton<IAutoVoiceService, AutoVoiceService>();

            services
                .AddTransient<IColorRoleService, ColorRoleService>()
                .AddTransient<IBotInformationService, BotInformationService>();
            
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
                
                // TODO: Consider moving UseCommandsNext here
                // Since this will be called only when DiscordClient is requested (which happens only in IHostedServices)
                // then that means it can be here.

                return client;
            });
            services.AddHostedService<DiscordService>();

            return services;
        }
    }
}