using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UrfRidersBot.Common.Configuration;

namespace UrfRidersBot.Common
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// <para>Registers a configuration instance which <see cref="T"/> will bind against.</para>
        /// <para>Section name will be the name of <see cref="T"/>.</para>
        /// </summary>
        public static IServiceCollection ConfigureSection<T>(
            this IServiceCollection services, IConfiguration configuration) where T : class
        {
            return services.Configure<T>(configuration.GetSection(typeof(T).Name));
        }

        /// <summary>
        /// Configures and adds <see cref="DiscordClient"/> and <see cref="DiscordGuild"/> to the container.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHostBuilder ConfigureDiscord(this IHostBuilder builder)
        {
            return builder.ConfigureServices((context, services) =>
            {
                // Register DiscordClient
                services.AddSingleton(provider =>
                {
                    var discordOptions = provider.GetRequiredService<IOptions<Discord>>().Value;
                    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

                    return new DiscordClient(new DiscordConfiguration
                    {
                        Intents = DiscordIntents.All,
                        TokenType = TokenType.Bot,
                        Token = discordOptions.Token,
                        LoggerFactory = loggerFactory,
                        AlwaysCacheMembers = true
                    });
                });

                // Register DiscordGuild
                services.AddTransient(provider =>
                {
                    var client = provider.GetRequiredService<DiscordClient>();
                    var discordOptions = provider.GetRequiredService<IOptions<Discord>>().Value;

                    return client.Guilds[discordOptions.GuildId];
                });
            });
        }
    }
}