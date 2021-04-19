using System.Text.RegularExpressions;
using DSharpPlus;
using DSharpPlus.Entities;

namespace UrfRidersBot.Core.Common
{
    public static class DiscordEmojiHelper
    {
        private static readonly Regex GuildEmoteRegex;
        
        static DiscordEmojiHelper()
        {
            GuildEmoteRegex = new Regex(@"^<a?:[a-zA-Z0-9_]+?:(?<id>\d+)>$", RegexOptions.Compiled);
        }
        
        /// <summary>
        /// Parses the emoji in either the guild emote format or in unicode. Named emojis are not supported here.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="emojiText">Raw emoji in either guild emote format (&lt;:name:id&gt;) or in unicode.</param>
        public static DiscordEmoji? Parse(DiscordClient client, string emojiText)
        {
            var match = GuildEmoteRegex.Match(emojiText);
            if (match.Success)
            {
                return ParseGuildEmote(client, match);
            }

            if (!DiscordEmoji.IsValidUnicode(emojiText))
            {
                return null;
            }

            return DiscordEmoji.FromUnicode(client, emojiText);
        }

        private static DiscordEmoji? ParseGuildEmote(DiscordClient client, Match match)
        {
            if (!ulong.TryParse(match.Groups["id"].Value, out var id))
            {
                return null;
            }

            return DiscordEmoji.FromGuildEmote(client, id);
        }
    }
}