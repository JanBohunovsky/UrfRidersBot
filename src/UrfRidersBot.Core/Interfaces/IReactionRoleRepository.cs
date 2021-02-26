using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Entities;

namespace UrfRidersBot.Core.Interfaces
{
    public interface IReactionRoleRepository
    {
        ValueTask<DiscordRole?> GetRoleAsync(DiscordMessage message, DiscordEmoji emoji);
        ValueTask<DiscordEmoji?> GetEmojiAsync(DiscordClient client, DiscordMessage message, DiscordRole role);
        Task AddAsync(ReactionRole reactionRole);
        void Remove(ReactionRole reactionRole);
    }
}