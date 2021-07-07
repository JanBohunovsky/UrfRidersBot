using System.Collections.Generic;
using System.Linq;
using DSharpPlus;
using DSharpPlus.Entities;
using UrfRidersBot.Common.Commands.Entities;

namespace UrfRidersBot.Common.Commands.Extensions
{
    public static class CommandDefinitionExtensions
    {
        public static IEnumerable<DiscordApplicationCommand> ToDiscord(this IEnumerable<CommandDefinition> commands)
        {
            commands = commands.ToList();
            var result = new List<DiscordApplicationCommand>();

            result.AddRange(GetSimpleCommands(commands));
            result.AddRange(GetSimpleGroups(commands));
            result.AddRange(GetNestedGroups(commands));

            return result;
        }

        /// <summary>
        /// Returns commands that are not in any group.
        /// </summary>
        private static IEnumerable<DiscordApplicationCommand> GetSimpleCommands(IEnumerable<CommandDefinition> commands)
        {
            foreach (var command in commands.Where(c => c.Parent is null))
            {
                var parameters = command.Parameters?.Values.Select(p => p.ToDiscord());
                yield return new DiscordApplicationCommand(command.Name, command.Description, parameters);
            }
        }

        /// <summary>
        /// Returns command groups that are not in any group.
        /// </summary>
        private static IEnumerable<DiscordApplicationCommand> GetSimpleGroups(IEnumerable<CommandDefinition> commands)
        {
            var groups = commands.Where(c => c.Parent is not null && c.Parent.Parent is null)
                .GroupBy(c => c.Parent!);
            
            foreach (var groupedCommands in groups)
            {
                var group = groupedCommands.Key;
                var subCommands = groupedCommands.Select(c => c.ToDiscordSubCommand());
                yield return new DiscordApplicationCommand(group.Name, group.Description, subCommands);
            }
        }

        /// <summary>
        /// Returns command groups that have sub-groups.
        /// </summary>
        private static IEnumerable<DiscordApplicationCommand> GetNestedGroups(IEnumerable<CommandDefinition> commands)
        {
            var groups = commands.Where(c => c.Parent?.Parent is not null)
                .GroupBy(c => c.Parent!.Parent!);

            foreach (var groupedCommands in groups)
            {
                var group = groupedCommands.Key;
                var subGroups = groupedCommands.GroupBy(c => c.Parent!)
                    .Select(g =>
                    {
                        var subGroup = g.Key;
                        var subCommands = g.Select(c => c.ToDiscordSubCommand());

                        return new DiscordApplicationCommandOption(
                            subGroup.Name,
                            subGroup.Description,
                            ApplicationCommandOptionType.SubCommandGroup,
                            options: subCommands
                        );
                    });

                yield return new DiscordApplicationCommand(group.Name, group.Description, subGroups);
            }
        }
        
    }
}