using DSharpPlus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

        public static IServiceCollection AddOnReadyService<TService>(this IServiceCollection services)
            where TService : class, IOnReadyService
        {
            return services
                .AddSingleton<TService>()
                .AddSingleton<IOnReadyService>(provider => provider.GetRequiredService<TService>());
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddSingleton<EmbedService>();

            services
                .AddFetchableHostedService<InteractiveService>();

            return services;
        }

        public static IServiceCollection AddConfiguration(this IServiceCollection services)
        {
            return services
                .AddSingleton<BotConfiguration>()
                .AddOnReadyService<EmoteConfiguration>()
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

            services.AddSingleton(provider =>
            {
                var secrets = provider.GetRequiredService<SecretsConfiguration>();
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                
                var client = new DiscordClient(new DiscordConfiguration
                {
                    Intents = DiscordIntents.All,
                    TokenType = TokenType.Bot,
                    Token = secrets.DiscordToken,
                    LoggerFactory = loggerFactory,
                    AlwaysCacheMembers = true
                });

                return client;
            });
            services.AddHostedService<DiscordService>();

            return services;
        }
    }
}