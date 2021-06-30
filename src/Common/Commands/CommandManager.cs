using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Common.Commands.Built;

namespace UrfRidersBot.Common.Commands
{
    public class CommandManager
    {
        private readonly ILogger<CommandManager> _logger;

        public CommandManager(ILogger<CommandManager> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Builds all available commands in <paramref name="assembly"/>.
        /// </summary>
        public IEnumerable<CommandDefinition> BuildCommands(Assembly assembly)
        {
            var commandTypes = assembly.ExportedTypes
                .Where(t => !t.IsInterface && !t.IsAbstract && t.GetInterfaces().Any(ti => ti == typeof(ICommand)));

            return BuildCommands(commandTypes);
        }

        /// <summary>
        /// Builds the specified commands.
        /// </summary>
        public IEnumerable<CommandDefinition> BuildCommands(params ICommand[] commands)
        {
            return BuildCommands(commands.Select(c => c.GetType()));
        }

        private IEnumerable<CommandDefinition> BuildCommands(IEnumerable<Type> commandTypes)
        {
            return commandTypes.Select(commandType => new CommandDefinition(commandType));
        }
    }
}