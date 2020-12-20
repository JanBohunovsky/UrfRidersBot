using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UrfRidersBot.Library
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUrfRidersBot(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuration
            services
                .AddSingleton<BotConfiguration>()
                .AddSingleton<EmoteConfiguration>();

            return services;
        }
    }
}