using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Core.Commands.Helpers;
using UrfRidersBot.Core.Commands.Services;

namespace UrfRidersBot.Infrastructure.Commands.Services
{
    internal class SlashCommandHostedService : IHostedService
    {
        private readonly DiscordClient _client;
        private readonly ICommandManager _commandManager;
        private readonly ICommandHandler _commandHandler;
        private readonly ILogger<SlashCommandHostedService> _logger;

        public SlashCommandHostedService(
            DiscordClient client,
            ICommandManager commandManager,
            ICommandHandler commandHandler,
            ILogger<SlashCommandHostedService> logger)
        {
            _client = client;
            _commandManager = commandManager;
            _commandHandler = commandHandler;
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

            var discordCommands = DiscordApplicationCommandHelper.FromSlashCommands(commands);
            _client.BulkOverwriteGuildApplicationCommandsAsync(637650172083437579, discordCommands);

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