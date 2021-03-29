using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UrfRidersBot.Core.Commands.Built;

namespace UrfRidersBot.Infrastructure.Commands.Models
{
    internal class CommandCreatePayload
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("description")]
        public string Description { get; set; } = "";

        [JsonProperty("options")]
        public List<DiscordCommandOption>? Options { get; set; }

        public static List<CommandCreatePayload> FromCommands(List<SlashCommand> commands)
        {
            var result = new List<CommandCreatePayload>();
            
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

        private static IEnumerable<CommandCreatePayload> FromBaseCommands(IEnumerable<SlashCommand> baseCommands)
        {
            return baseCommands.Select(c => new CommandCreatePayload
            {
                Name = c.Name,
                Description = c.Description,
                Options = c.Parameters?.Values
                    .Select(DiscordCommandOption.FromParameter)
                    .ToList()
            });
        }

        private static IEnumerable<CommandCreatePayload> FromCommandGroups(IEnumerable<CommandGroup> commandGroups)
        {
            return commandGroups.Select(g => new CommandCreatePayload
            {
                Name = g.Name,
                Description = g.Description,
                Options = g.SubCommands
                    .Select(DiscordCommandOption.FromSubCommand)
                    .ToList()
            });
        }

        private static IEnumerable<CommandCreatePayload> FromSuperCommandGroups(IEnumerable<SuperCommandGroup> superCommandGroups)
        {
            return superCommandGroups.Select(sg => new CommandCreatePayload
            {
                Name = sg.Name,
                Description = sg.Description,
                Options = sg.SubGroups
                    .Select(g => DiscordCommandOption.FromSubCommandGroup(g.Name, g.Description, g.SubCommands))
                    .ToList()
            });
        }

        private class CommandGroup
        {
            public string Name { get; }
            public string Description { get; }
            public List<SlashCommand> SubCommands { get; }

            public CommandGroup(SlashCommandGroup group, IEnumerable<SlashCommand> subCommands)
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