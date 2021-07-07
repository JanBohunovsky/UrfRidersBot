using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UrfRidersBot.Common.Commands.Entities;
using UrfRidersBot.Common.Commands.Extensions;
using UrfRidersBot.Common.Configuration;

namespace UrfRidersBot.Common.Commands
{
    public class CommandManager
    {
        private readonly DiscordClient _client;
        private readonly IOptionsMonitor<Discord> _options;
        private readonly ILogger<CommandManager> _logger;
        private readonly Dictionary<string, CommandDefinition> _commands;

        public CommandManager(DiscordClient client, IOptionsMonitor<Discord> options, ILogger<CommandManager> logger)
        {
            _client = client;
            _options = options;
            _logger = logger;
            _commands = new Dictionary<string, CommandDefinition>();
        }

        public void AddCommandsFromAssembly(Assembly assembly)
        {
            var commandTypes = assembly.ExportedTypes
                .Where(t => !t.IsInterface && !t.IsAbstract && t.GetInterfaces().Any(ti => ti == typeof(ICommand)));

            foreach (var commandType in commandTypes)
            {
                AddCommand(commandType);
            }
        }

        public void AddCommand<T>() where T : ICommand
        {
            AddCommand(typeof(T));
        }

        private void AddCommand(Type commandType)
        {
            var command = new CommandDefinition(commandType);
            if (!_commands.TryAdd(command.FullName, command))
            {
                throw new InvalidOperationException($"A command with this full name ({command.FullName}) has already been registered: {commandType.FullName}");
            }
        }

        public void ClearCommands()
        {
            _commands.Clear();
        }

        /// <summary>
        /// Registers all commands, that were added to this manager, as Slash commands to Discord.
        /// </summary>
        public async Task RegisterCommandsAsync()
        {
            if (!_commands.Any())
            {
                throw new InvalidOperationException("No commands to register.");
            }
            
            var discordCommands = _commands.Values.ToDiscord();
            await _client.BulkOverwriteGuildApplicationCommandsAsync(_options.CurrentValue.GuildId, discordCommands);
            
            GroupDefinition.Cache.Clear();
        }

        public CommandDefinition FindCommand(string fullName)
        {
            return _commands[fullName];
        }
    }
}