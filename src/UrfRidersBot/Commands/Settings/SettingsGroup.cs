using UrfRidersBot.Core.Commands;
using UrfRidersBot.Infrastructure.Commands.Checks;

namespace UrfRidersBot.Commands.Settings
{
    [RequireGuildRank(GuildRank.Admin)]
    public class SettingsGroup : ICommandGroup
    {
        public string Name => "settings";
        public string Description => "Read and modify a server specific variables.";
    }
}