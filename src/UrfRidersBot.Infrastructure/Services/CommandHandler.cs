using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Commands.Entities;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Infrastructure
{
    internal class CommandHandler : ICommandHandler
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordClient _client;
        private readonly IInteractionService _service;
        private readonly ILogger<CommandHandler> _logger;
        private Dictionary<string, SlashCommand> _commands;

        public CommandHandler(
            IServiceProvider provider,
            DiscordClient client,
            IInteractionService service,
            ILogger<CommandHandler> logger)
        {
            _provider = provider;
            _client = client;
            _service = service;
            _logger = logger;
            _commands = new Dictionary<string, SlashCommand>();
        }

        public void SetCommands(IEnumerable<SlashCommand> commands)
        {
            _commands = commands.ToDictionary(GetFullCommandName, c => c);
        }

        private string GetFullCommandName(SlashCommand command)
        {
            var sb = new StringBuilder();

            if (command.Parent is not null)
            {
                var commandGroup = command.Parent;
                if (commandGroup.Parent is not null)
                {
                    var groupParent = commandGroup.Parent;
                    sb.Append(groupParent.Name);
                    sb.Append(' ');
                }

                sb.Append(commandGroup.Name);
                sb.Append(' ');
            }

            sb.Append(command.Name);
            
            return sb.ToString();
        }

        public async Task HandleAsync(DiscordInteraction interaction)
        {
            var request = ParseCommand(interaction.Data);

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

            if (request.Parameters is not null && command.Parameters is not null)
            {
                foreach (var requestParameter in request.Parameters)
                {
                    var parameter = command.Parameters[requestParameter.Name];
                    var value = await GetParameterValueAsync(requestParameter, interaction);
                    parameter.Property.SetValue(instance, value);
                }
            }

            var context = new CommandContext(_client, interaction, _service);

            try
            {
                await instance.HandleAsync(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Command '{Command}' threw an exception", request.FullName);
            }
        }

        private CommandRequest ParseCommand(DiscordInteractionData data)
        {
            var sb = new StringBuilder();

            sb.Append(data.Name);

            var option = data.Options?.First();
            if (option is null)
            {
                return new CommandRequest(sb.ToString(), null);
            }
            
            if (option.Type == ApplicationCommandOptionType.SubCommand)
            {
                sb.Append(' ');
                sb.Append(option.Name);

                return new CommandRequest(sb.ToString(), option.Options);
            }

            if (option.Type == ApplicationCommandOptionType.SubCommandGroup)
            {
                // Append sub-group name
                sb.Append(' ');
                sb.Append(option.Name);
                
                // Append command name
                var command = option.Options.First();
                sb.Append(' ');
                sb.Append(command.Name);

                return new CommandRequest(sb.ToString(), command.Options);
            }

            return new CommandRequest(sb.ToString(), data.Options);
        }

        private async ValueTask<object?> GetParameterValueAsync(DiscordInteractionDataOption parameter, DiscordInteraction interaction)
        {
            object? result;
            switch (parameter.Type)
            {
                case ApplicationCommandOptionType.String:
                    result = parameter.Value.ToString();
                    break;
                case ApplicationCommandOptionType.Integer:
                    result = (long)parameter.Value;
                    break;
                case ApplicationCommandOptionType.Boolean:
                    result = (bool)parameter.Value;
                    break;
                case ApplicationCommandOptionType.User:
                    var userId = (ulong)parameter.Value;
                    if (interaction.Data.Resolved.Members.TryGetValue(userId, out var member))
                    {
                        result = member;
                    }
                    else if (interaction.Data.Resolved.Users.TryGetValue(userId, out var user))
                    {
                        result = user;
                    }
                    else
                    {
                        result = await _client.GetUserAsync(userId);
                    }
                    break;
                case ApplicationCommandOptionType.Channel:
                    var channelId = (ulong)parameter.Value;
                    if (interaction.Data.Resolved.Channels.TryGetValue(channelId, out var channel))
                    {
                        result = channel;
                    }
                    else
                    {
                        result = interaction.Guild.GetChannel(channelId);
                    }
                    break;
                case ApplicationCommandOptionType.Role:
                    var roleId = (ulong)parameter.Value;
                    if (interaction.Data.Resolved.Roles.TryGetValue(roleId, out var role))
                    {
                        result = role;
                    }
                    else
                    {
                        result = interaction.Guild.GetRole(roleId);
                    }
                    break;
                default:
                    throw new InvalidOperationException($"Invalid parameter type: '{parameter.Type}'");
            }

            return result;
        }
        
        private class CommandRequest
        {
            public string FullName { get; }
            public List<DiscordInteractionDataOption>? Parameters { get; }

            public CommandRequest(string fullName, IEnumerable<DiscordInteractionDataOption>? parameters)
            {
                FullName = fullName;
                Parameters = parameters?.ToList();
            }
        }
    }
}