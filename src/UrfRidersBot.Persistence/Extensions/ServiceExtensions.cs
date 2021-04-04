using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UrfRidersBot.Core.AutoVoice;
using UrfRidersBot.Core.Common;
using UrfRidersBot.Persistence.AutoVoice;
using UrfRidersBot.Persistence.Common;

namespace UrfRidersBot.Persistence
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
        {
            services
                .AddRepositories()
                .AddDbContext(connectionString)
                .AddTransient<IUnitOfWork, UnitOfWork>()
                .AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();

            return services;
        }
        
        private static IServiceCollection AddDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<UrfRidersDbContext>(
                options => options.UseNpgsql(connectionString),
                ServiceLifetime.Transient,
                ServiceLifetime.Transient
            );
            services.AddDbContextFactory<UrfRidersDbContext>(
                options => options.UseNpgsql(connectionString)
            );

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // TODO: Use reflection
            services
                .AddTransient<IAutoVoiceSettingsRepository, AutoVoiceSettingsRepository>();

            return services;
        }
    }
}