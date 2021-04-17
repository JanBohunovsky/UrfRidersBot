using DSharpPlus.Entities;

namespace UrfRidersBot.Core.ReactionRoles
{
    public class ReactionRole
    {
        public DiscordMessage Message { get; }
        public DiscordRole Role { get; }
        public DiscordEmoji Emoji { get; }

        public ReactionRole(DiscordMessage message, DiscordRole role, DiscordEmoji emoji)
        {
            Message = message;
            Role = role;
            Emoji = emoji;
        }
    }
}