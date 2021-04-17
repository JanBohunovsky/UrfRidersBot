using DSharpPlus.Entities;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Core.ReactionRoles
{
    public interface IReactionRoleRepository : IRepository
    {
        DiscordRole? GetRole(DiscordMessage message, DiscordEmoji emoji);
        DiscordEmoji? GetEmoji(DiscordMessage message, DiscordRole role);
        bool Add(ReactionRole reactionRole);
        void Remove(DiscordMessage message, DiscordRole role);
    }
}