using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Commands.Built;
using UrfRidersBot.Core.Commands.Services;
using UrfRidersBot.Infrastructure.Commands.Models;

namespace UrfRidersBot.Infrastructure.Commands.Services
{
    internal class CommandHandler : ICommandHandler
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordClient _client;
        private readonly IInteractionService _interactionService;
        private readonly ILogger<CommandHandler> _logger;
        private Dictionary<string, SlashCommand> _commands;

        public CommandHandler(
            IServiceProvider provider,
            DiscordClient client,
            IInteractionService interactionService,
            ILogger<CommandHandler> logger)
        {
            _provider = provider;
            _client = client;
            _interactionService = interactionService;
            _logger = logger;
            _commands = new Dictionary<string, SlashCommand>();
        }

        public void SetCommands(IEnumerable<SlashCommand> commands)
        {
            _commands = commands.ToDictionary(c => c.GetFullName(), c => c);
        }

        public async Task HandleAsync(DiscordInteraction interaction)
        {
            var request = CommandRequest.FromInteractionData(interaction.Data);

            if (!_commands.ContainsKey(request.FullName))
            {
                _logger.LogError("An interaction was created, but no command was registered for it: '{Command}'",
                    request.FullName);
                return;
            }

            var command = _commands[request.FullName];

            using var scope = _provider.CreateScope();
            if (ActivatorUtilities.CreateInstance(scope.ServiceProvider, command.Class) is not ICommand instance)
            {
                _logger.LogError("Could not create an instance of a type {Type} for a command '{Command}'",
                    command.Class, request.FullName);
                return;
            }

            if (request.Parameters is not null)
            {
                command.FillParameters(instance, request.Parameters, interaction.Data.Resolved);
            }

            var context = new CommandContext(_client, interaction, _interactionService);

            var checkResult = await command.RunChecksAsync(context, _provider);
            if (!checkResult.IsSuccessful)
            {
                await context.CreateEphemeralResponseAsync(checkResult.Reason!);
                _logger.LogWarning("Checks failed for command '{Command}': {Checks}", 
                    command.Class.Name, checkResult.Reason);
                return;
            }

            try
            {
                await instance.HandleAsync(context);
            }
            catch (Exception e)
            {
                await context.CreateEphemeralResponseAsync($"Command failed, please contact bot owner.\n" +
                                                           $"Exception: `{e.Message}`");
                _logger.LogError(e, "Command '{Command}' threw an exception", command.Class.Name);
            }
        }
    }
}