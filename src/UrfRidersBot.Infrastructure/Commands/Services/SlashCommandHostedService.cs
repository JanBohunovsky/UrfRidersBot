using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Hosting;
using UrfRidersBot.Core.Commands.Services;

namespace UrfRidersBot.Infrastructure.Commands.Services
{
    internal class SlashCommandHostedService : IHostedService
    {
        private readonly DiscordClient _client;
        private readonly ICommandManager _commandManager;
        private readonly ICommandHandler _commandHandler;
        private readonly IInteractionService _service;

        public SlashCommandHostedService(
            DiscordClient client,
            ICommandManager commandManager,
            ICommandHandler commandHandler,
            IInteractionService service)
        {
            _client = client;
            _commandManager = commandManager;
            _commandHandler = commandHandler;
            _service = service;
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
            
            _commandHandler.SetCommands(commands);

            _ = _service.RegisterCommandsAsync(commands, 637650172083437579);

            return Task.CompletedTask;
        }

        private Task OnInteractionCreated(DiscordClient sender, InteractionCreateEventArgs e)
        {
            _ = _commandHandler.HandleAsync(e.Interaction);

            return Task.CompletedTask;
        }
    }

}