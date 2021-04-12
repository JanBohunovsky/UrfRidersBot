using UrfRidersBot.Core.Commands;
using UrfRidersBot.Infrastructure.Commands.Checks;

namespace UrfRidersBot.Commands.ReactionRoles
{
    [RequireGuildRank(GuildRank.Admin)]
    public class ReactionRolesGroup : ICommandGroup
    {
        public string Name => "reaction-roles";
        public string Description => "Set up self-assignable roles.";
    }
}