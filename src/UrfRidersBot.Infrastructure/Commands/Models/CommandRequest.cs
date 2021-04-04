using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;

namespace UrfRidersBot.Infrastructure.Commands.Models
{
    internal class CommandRequest
    {
        public string FullName { get; }
        public List<DiscordInteractionDataOption>? Parameters { get; }

        public CommandRequest(string fullName, IEnumerable<DiscordInteractionDataOption>? parameters)
        {
            FullName = fullName;
            Parameters = parameters?.ToList();
        }

        public static CommandRequest FromInteractionData(DiscordInteractionData data)
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
    }
}