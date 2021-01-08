using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace UrfRidersBot.Interactive
{
    public interface IReactionHandler
    {
        Task ReactionAdded(DiscordMessage message, DiscordUser user, DiscordEmoji emote);
        Task ReactionRemoved(DiscordMessage message, DiscordUser user, DiscordEmoji emote);
    }
}