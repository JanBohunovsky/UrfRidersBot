using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Core.Commands.Entities;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Infrastructure
{
    internal class CommandHandler : ICommandHandler
    {
        private readonly ILogger<CommandHandler> _logger;
        private Dictionary<string, SlashCommand> _commands;

        public CommandHandler(ILogger<CommandHandler> logger)
        {
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
            throw new NotImplementedException();
        }
    }
}