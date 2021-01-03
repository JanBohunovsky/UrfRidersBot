using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace UrfRidersBot
{
    public class DiscordService : IHostedService
    {
        private readonly DiscordSocketClient _client;
        private readonly SecretsConfiguration _secrets;
        private readonly CommandService _commandService;
        private readonly CommandHandlingService _commandHandlingService;
        private readonly IDbContextFactory<UrfRidersDbContext> _dbContextFactory;
        private readonly ILogger<DiscordSocketClient> _discordLogger;
        private readonly ILogger<CommandService> _commandLogger;
        private readonly ILogger<DiscordService> _logger;

        public DiscordService(
            DiscordSocketClient client,
            SecretsConfiguration secrets,
            CommandService commandService,
            CommandHandlingService commandHandlingService,
            IDbContextFactory<UrfRidersDbContext> dbContextFactory,
            ILoggerFactory loggerFactory)
        {
            _client = client;
            _secrets = secrets;
            _commandService = commandService;
            _commandHandlingService = commandHandlingService;
            _dbContextFactory = dbContextFactory;

            _discordLogger = loggerFactory.CreateLogger<DiscordSocketClient>();
            _commandLogger = loggerFactory.CreateLogger<CommandService>();
            _logger = loggerFactory.CreateLogger<DiscordService>();

            _client.Log += OnDiscordLog;
            _commandService.Log += OnCommandLog;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Add command modules to discord
            await _commandHandlingService.RegisterCommands(Assembly.GetEntryAssembly());

            // Make sure the database is on the latest migration
            await using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                _logger.LogInformation("Migrating database...");
                await dbContext.Database.MigrateAsync(cancellationToken);
            }
            
            // Start discord client
            await _client.LoginAsync(TokenType.Bot, _secrets.DiscordToken);
            await _client.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.LogoutAsync();
            await _client.StopAsync();

            _client.Log -= OnDiscordLog;
            _commandService.Log -= OnCommandLog;
        }
        
        private static LogLevel LogLevelFromSeverity(LogSeverity severity) => (LogLevel)Math.Abs((int)severity - 5);
        
        private Task OnDiscordLog(LogMessage message)
        {
            _discordLogger.Log(
                LogLevelFromSeverity(message.Severity),
                0,
                message,
                message.Exception,
                (m, e) => message.ToString(prependTimestamp: false)
            );

            return Task.CompletedTask;
        }
        
        private Task OnCommandLog(LogMessage message)
        {
            _commandLogger.Log(
                LogLevelFromSeverity(message.Severity),
                0,
                message,
                message.Exception,
                (m, e) => message.ToString(prependTimestamp: false)
            );

            return Task.CompletedTask;
        }
    }
}