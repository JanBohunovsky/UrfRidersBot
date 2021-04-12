using UrfRidersBot.Core.Commands;
using UrfRidersBot.Infrastructure.Commands.Checks;

namespace UrfRidersBot.Commands.Color
{
    [RequireGuildRank(GuildRank.Member)]
    public class ColorGroup : ICommandGroup
    {
        public string Name => "color";
        public string Description => "Manage your own role with custom color.";
    }
}