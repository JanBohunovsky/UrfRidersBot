using Discord;

namespace UrfRidersBot
{
    public static class StringExtensions
    {
        /// <summary>
        /// Parses <see cref="value"/> into either <see cref="Emote"/> or <see cref="Emoji"/>.
        /// Note: If the <see cref="value"/> isn't a guild emote, then it will just return an <see cref="Emoji"/>
        /// and this CAN be invalid emoji if the <see cref="value"/> doesn't contain... emoji.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEmote ToEmote(this string value)
        {
            if (Emote.TryParse(value, out Emote emote))
                return emote;

            return new Emoji(value);
        }

        /// <summary>
        /// Returns back the <see cref="text"/> in an in-line code block.
        /// </summary>
        /// <param name="text">The text to be wrapped in an in-line code block.</param>
        /// <returns>Text in a code block or null if <see cref="text"/> is null.</returns>
        public static string? ToCode(this string? text)
        {
            return text == null ? null : $"`{text}`";
        }
    }
}