using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace UrfRidersBot
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddFetchableHostedService<TService>(this IServiceCollection services)
            where TService : class, IHostedService
        {
            return services
                .AddSingleton<TService>()
                .AddHostedService(provider => provider.GetRequiredService<TService>());
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddTransient<EmbedService>()
                .AddTransient<HelpService>();

            services
                .AddFetchableHostedService<InteractiveService>();

            return services;
        }

        public static IServiceCollection AddConfiguration(this IServiceCollection services)
        {
            return services
                .AddSingleton<BotConfiguration>()
                .AddSingleton<EmoteConfiguration>()
                .AddSingleton<SecretsConfiguration>();
        }

        public static IServiceCollection AddUrfRidersDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<UrfRidersDbContext>(
                options => options.UseNpgsql(connectionString),
                ServiceLifetime.Transient,
                ServiceLifetime.Transient
            );
            services.AddDbContextFactory<UrfRidersDbContext>(options => options.UseNpgsql(connectionString));
            
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
                .AddSingleton<CommandHandlingService>();

            services.AddHostedService<DiscordService>();

            return services;
        }
    }
}