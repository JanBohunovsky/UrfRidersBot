using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UrfRidersBot.Library.Internal.HostedServices;
using UrfRidersBot.Library.Internal.Services;

namespace UrfRidersBot.Library
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUrfRidersBot(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuration
            services
                .AddSingleton<BotConfiguration>()
                .AddSingleton<EmoteConfiguration>()
                .AddSingleton<SecretsConfiguration>();

            // Data Access
            services.AddDbContext<UrfRidersDbContext>(
                options => options.UseNpgsql(configuration.GetConnectionString("UrfRidersData")),
                ServiceLifetime.Transient,
                ServiceLifetime.Transient
            );
            services.AddDbContextFactory<UrfRidersDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("UrfRidersData")));

            // Services
            services
                .AddTransient<IEmbedService, EmbedService>()
                .AddTransient<IHelpService, HelpService>();

            services
                .AddSingleton<IInteractiveService, InteractiveService>();
            
            // Hosted Services
            services.AddHostedService<InteractiveHostedService>();

            return services;
        }

        public static IServiceCollection AddDiscord(this IServiceCollection services)
        {
            var discordClient = new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                GatewayIntents =
                    GatewayIntents.Guilds |
                    GatewayIntents.GuildMembers |
                    GatewayIntents.GuildBans |
                    GatewayIntents.GuildEmojis |
                    GatewayIntents.GuildIntegrations |
                    GatewayIntents.GuildWebhooks |
                    GatewayIntents.GuildInvites |
                    GatewayIntents.GuildVoiceStates |
                    GatewayIntents.GuildPresences |
                    GatewayIntents.GuildMessages |
                    GatewayIntents.GuildMessageReactions |
                    GatewayIntents.GuildMessageTyping |
                    GatewayIntents.DirectMessages |
                    GatewayIntents.DirectMessageReactions |
                    GatewayIntents.DirectMessageTyping,
            });

            var commandService = new CommandService(new CommandServiceConfig
            {
                IgnoreExtraArgs = false,
            });

            services
                .AddSingleton(discordClient)
                .AddSingleton(commandService)
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<DiscordLogService>();

            services.AddHostedService<DiscordHostedService>();

            return services;
        }
    }
}