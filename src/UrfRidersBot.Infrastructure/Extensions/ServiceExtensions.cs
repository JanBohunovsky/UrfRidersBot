using System;
using System.Net.Http;
using System.Net.Http.Headers;
using DSharpPlus;
using DSharpPlus.Entities;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UrfRidersBot.Core.AutoVoice;
using UrfRidersBot.Core.ColorRole;
using UrfRidersBot.Core.Commands.Services;
using UrfRidersBot.Core.Common;
using UrfRidersBot.Core.Common.Configuration;
using UrfRidersBot.Core.ReactionRoles;
using UrfRidersBot.Core.Settings;
using UrfRidersBot.Infrastructure.AutoVoice;
using UrfRidersBot.Infrastructure.ColorRole;
using UrfRidersBot.Infrastructure.Commands.Services;
using UrfRidersBot.Infrastructure.Common;
using UrfRidersBot.Infrastructure.ReactionRoles;
using UrfRidersBot.Infrastructure.Settings;

namespace UrfRidersBot.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, IConfiguration configuration, string connectionString)
        {
            services.AddConfigurations(configuration);
            services.AddPersistence(connectionString);
            services.AddDiscordBot();
            services.AddSlashCommands();

            return services;
        }

        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DiscordOptions>(configuration.GetSection(DiscordOptions.SectionName));
            services.Configure<RiotGamesOptions>(configuration.GetSection(RiotGamesOptions.SectionName));

            return services;
        }
        
        public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
        {
            BsonMapper.Global.UseLowerCaseDelimiter();
             
            services.AddRepositories();
            services.AddTransient(_ => new LiteDatabase(connectionString));

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // TODO: Use reflection
            services.AddTransient<IGuildSettingsRepository, GuildSettingsRepository>();
            services.AddTransient<IAutoVoiceSettingsRepository, AutoVoiceSettingsRepository>();
            services.AddTransient<IColorRoleRepository, ColorRoleRepository>();
            services.AddTransient<IReactionRoleRepository, ReactionRoleRepository>();

            return services;
        }

        public static IServiceCollection AddSlashCommands(this IServiceCollection services)
        {
            services.AddSingleton(CreateDiscordHttpClient);
            services.AddSingleton<ICommandManager, CommandManager>();
            services.AddSingleton<ICommandHandler, CommandHandler>();
            services.AddHostedService<SlashCommandHostedService>();

            return services;
        }

        private static HttpClient CreateDiscordHttpClient(IServiceProvider provider)
        {
            var discordOptions = provider.GetRequiredService<IOptions<DiscordOptions>>().Value;
            
            return new HttpClient
            {
                BaseAddress = new Uri("https://discord.com/api/v8/"),
                DefaultRequestHeaders =
                {
                    Accept =
                    {
                        new MediaTypeWithQualityHeaderValue("application/json")
                    },
                    Authorization = new AuthenticationHeaderValue("Bot", discordOptions.Token)
                }
            };
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

                return client;
            });
            
            services.AddHostedService<DiscordBot>();
            
            // The currently configured guild
            services.AddTransient<DiscordGuild>(provider =>
            {
                var client = provider.GetRequiredService<DiscordClient>();
                var options = provider.GetRequiredService<IOptions<DiscordOptions>>().Value;

                return client.Guilds[options.GuildId];
            });

            return services;
        }
    }
}