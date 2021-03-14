using DSharpPlus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UrfRidersBot.Core.Configuration;
using UrfRidersBot.Core.Interfaces;
using UrfRidersBot.Infrastructure.HostedServices;

namespace UrfRidersBot.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DiscordOptions>(configuration.GetSection(DiscordOptions.SectionName));
            services.Configure<RiotGamesOptions>(configuration.GetSection(RiotGamesOptions.SectionName));

            return services;
        }

        public static IServiceCollection AddDiscordBot(this IServiceCollection services)
        {
            // TODO: Automate this via reflection
            // Project services
            services
                .AddHostedService<AutoVoiceHostedService>()
                .AddHostedService<ReactionRolesHostedService>();
            
            services
                .AddSingleton<IAutoVoiceService, AutoVoiceService>()
                .AddSingleton<IBotInformationService, BotInformationService>();

            services
                .AddTransient<IColorRoleService, ColorRoleService>();
            
            // Discord client and service
            services.AddSingleton(provider =>
            {
                var discordOptions = provider.GetRequiredService<IOptions<DiscordOptions>>().Value;
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                
                var client = new DiscordClient(new DiscordConfiguration
                {
                    Intents = DiscordIntents.All,
                    TokenType = TokenType.Bot,
                    Token = discordOptions.Token,
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