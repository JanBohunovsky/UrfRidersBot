using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Core;
using UrfRidersBot.Core.Commands.Helpers;
using UrfRidersBot.Core.Commands.Services;

namespace UrfRidersBot.Infrastructure.Commands.Services
{
    internal class SlashCommandHostedService : IHostedService
    {
        private readonly DiscordClient _client;
        private readonly ICommandManager _commandManager;
        private readonly ICommandHandler _commandHandler;
        private readonly IHostEnvironment _environment;
        private readonly ILogger<SlashCommandHostedService> _logger;

        public SlashCommandHostedService(
            DiscordClient client,
            ICommandManager commandManager,
            ICommandHandler commandHandler,
            IHostEnvironment environment,
            ILogger<SlashCommandHostedService> logger)
        {
            _client = client;
            _commandManager = commandManager;
            _commandHandler = commandHandler;
            _environment = environment;
            _logger = logger;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _client.Ready += OnReady;
            _client.InteractionCreated += OnInteractionCreated;
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _client.Ready -= OnReady;
            _client.InteractionCreated -= OnInteractionCreated;
            
            return Task.CompletedTask;
        }

        private Task OnReady(DiscordClient sender, ReadyEventArgs e)
        {
            var commands = _commandManager.BuildCommands().ToList();
            _commandHandler.AddCommands(commands);

            // TODO: Implement better system
            var guildId = _environment.IsDevelopment()
                ? UrfRidersGuilds.Dev
                : UrfRidersGuilds.Main;
            
            var discordCommands = DiscordApplicationCommandHelper.FromSlashCommands(commands);
            _client.BulkOverwriteGuildApplicationCommandsAsync(guildId, discordCommands);

            return Task.CompletedTask;
        }

        private Task OnInteractionCreated(DiscordClient sender, InteractionCreateEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await _commandHandler.HandleAsync(e.Interaction);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Handling of interaction {InteractionId} failed", e.Interaction.Id);
                }
            });

            return Task.CompletedTask;
        }
    }

}