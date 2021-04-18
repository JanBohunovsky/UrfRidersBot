using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Core.ReactionRoles
{
    public interface IReactionRoleRepository : IRepository
    {
        ValueTask<DiscordRole?> GetRoleAsync(DiscordMessage message, DiscordEmoji emoji);
        ValueTask<DiscordEmoji?> GetEmojiAsync(DiscordMessage message, DiscordRole role);
        ValueTask<bool> AddAsync(ReactionRole reactionRole);
        Task RemoveAsync(DiscordMessage message, DiscordRole role);
    }
}