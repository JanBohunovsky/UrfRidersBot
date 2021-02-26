using DSharpPlus.Entities;

namespace UrfRidersBot.Core.Entities
{
    public class ReactionRole
    {
        public DiscordMessage Message { get; }
        public DiscordEmoji Emoji { get; }
        public DiscordRole Role { get; }

        public ReactionRole(DiscordMessage message, DiscordEmoji emoji, DiscordRole role)
        {
            Message = message;
            Emoji = emoji;
            Role = role;
        }
    }
}