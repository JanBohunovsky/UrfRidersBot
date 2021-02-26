using DSharpPlus;
using DSharpPlus.Entities;

namespace UrfRidersBot.Persistence.Mappers
{
    public static class EmojiMapper
    {
        public static DiscordEmoji ToDiscord(DiscordClient client, string rawEmoji)
        {
            if (ulong.TryParse(rawEmoji, out var emojiId))
            {
                return DiscordEmoji.FromGuildEmote(client, emojiId);
            }

            return DiscordEmoji.FromName(client, rawEmoji);
        }

        public static string FromDiscord(DiscordEmoji emoji)
        {
            if (emoji.Id != default)
            {
                return emoji.Id.ToString();
            }

            return emoji.GetDiscordName();
        }
    }
}