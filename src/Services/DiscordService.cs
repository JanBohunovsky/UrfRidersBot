using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace UrfRidersBot
{
    public partial class DiscordService : IHostedService
    {
        private readonly DiscordClient _client;
        private readonly CommandsNextExtension _commands;
        private readonly BotConfiguration _botConfig;
        private readonly EmbedService _embedService;
        private readonly IDbContextFactory<UrfRidersDbContext> _dbContextFactory;
        private readonly ILogger<DiscordService> _logger;

        public DiscordService(
            DiscordClient client,
            BotConfiguration botConfig,
            EmbedService embedService,
            IDbContextFactory<UrfRidersDbContext> dbContextFactory,
            ILogger<DiscordService> logger,
            IServiceProvider provider)
        {
            _client = client;
            _botConfig = botConfig;
            _embedService = embedService;
            _dbContextFactory = dbContextFactory;
            _logger = logger;

            _commands = _client.UseCommandsNext(new CommandsNextConfiguration
            {
                EnableDms = false,
                EnableMentionPrefix = true,
                Services = provider,
                PrefixResolver = PrefixResolver
            });
            
            _commands.SetHelpFormatter<UrfRidersHelpFormatter>();
            _commands.CommandErrored += OnCommandErrored;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Add command modules to discord
            _commands.RegisterCommands(Assembly.GetEntryAssembly());
            
            // Make sure the database is on the latest migration
            await using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                _logger.LogInformation("Migrating database...");
                await dbContext.Database.MigrateAsync(cancellationToken);
            }
            
            // Start discord client
            await _client.ConnectAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.DisconnectAsync();

            _commands.CommandErrored -= OnCommandErrored;
        }
    }
}