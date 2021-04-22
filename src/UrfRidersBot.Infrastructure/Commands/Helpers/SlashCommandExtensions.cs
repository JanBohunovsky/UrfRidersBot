using System.Collections.Generic;
using System.Linq;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Commands.Built;

namespace UrfRidersBot.Infrastructure.Commands.Helpers
{
    internal static class SlashCommandExtensions
    {
        // NOTE: This code IS ugly and will probably stay ugly.
        // 
        // That is because of my SlashCommand structure, I have commands as the root and then I can go up the tree
        // to look at the parents, but discord is using the reverse, they have simple commands and command groups
        // as a root and then they go down to sub-command groups and sub-commands.
        // 
        // All this method does is it converts my style into the discord style, that's why we need the list of commands
        // instead of a single command (because we need to figure out it's parent group and add commands to them).
        public static IEnumerable<DiscordApplicationCommand> ToDiscord(this List<SlashCommandDefinition> commands)
        {
            var result = new List<DiscordApplicationCommand>();
            
            // Commands that have no parent
            var baseCommands = commands
                .Where(c => c.Parent is null);
            result.AddRange(FromBaseCommands(baseCommands));

            // Commands that have a parent group but those groups don't have parents
            var commandGroups = commands
                .Where(c => c.Parent is not null && c.Parent.Parent is null)
                .GroupBy(c => c.Parent)
                .Select(g => new CommandGroup(g.Key!, g));
            result.AddRange(FromCommandGroups(commandGroups));

            // Commands that have a parent group and those groups also have parents
            var superCommandGroups = commands
                .Where(c => c.Parent?.Parent is not null)
                .GroupBy(c => c.Parent!.Parent)
                .Select(g =>
                    new SuperCommandGroup(
                        g.Key!,
                        g.GroupBy(c => c.Parent)
                            .Select(g1 => new CommandGroup(g1.Key!, g1))));
            result.AddRange(FromSuperCommandGroups(superCommandGroups));

            return result;
        }

        private static IEnumerable<DiscordApplicationCommand> FromBaseCommands(IEnumerable<SlashCommandDefinition> baseCommands)
        {
            return baseCommands.Select(c =>
                new DiscordApplicationCommand(
                    c.Name,
                    c.Description,
                    c.Parameters?.Values
                        .Select(DiscordApplicationCommandOptionHelper.FromSlashCommandParameter)
                )
            );
        }

        private static IEnumerable<DiscordApplicationCommand> FromCommandGroups(IEnumerable<CommandGroup> commandGroups)
        {
            return commandGroups.Select(g =>
                new DiscordApplicationCommand(
                    g.Name,
                    g.Description,
                    g.SubCommands
                        .Select(DiscordApplicationCommandOptionHelper.FromSlashSubCommand)
                )
            );
        }

        private static IEnumerable<DiscordApplicationCommand> FromSuperCommandGroups(IEnumerable<SuperCommandGroup> superCommandGroups)
        {
            return superCommandGroups.Select(sg =>
                new DiscordApplicationCommand(
                    sg.Name,
                    sg.Description,
                    sg.SubGroups
                        .Select(g =>
                            DiscordApplicationCommandOptionHelper.FromSlashSubCommandGroup(
                                g.Name,
                                g.Description,
                                g.SubCommands
                            )
                        )
                )
            );
        }

        private class CommandGroup
        {
            public string Name { get; }
            public string Description { get; }
            public List<SlashCommandDefinition> SubCommands { get; }

            public CommandGroup(SlashCommandGroup group, IEnumerable<SlashCommandDefinition> subCommands)
            {
                Name = group.Name;
                Description = group.Description;
                SubCommands = subCommands.ToList();
            }
        }

        private class SuperCommandGroup
        {
            public string Name { get; }
            public string Description { get; }
            public List<CommandGroup> SubGroups { get; }

            public SuperCommandGroup(SlashCommandGroup group, IEnumerable<CommandGroup> subGroups)
            {
                Name = group.Name;
                Description = group.Description;
                SubGroups = subGroups.ToList();
            }
        }
    }
}