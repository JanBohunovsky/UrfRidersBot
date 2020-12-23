using Discord;

namespace UrfRidersBot.Library
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
        /// <para>
        /// You can specify custom null text if the <see cref="text"/> is null. If you don't then this method returns null.
        /// </para>
        /// </summary>
        /// <param name="text">The text to be wrapped in an in-line code block.</param>
        /// <param name="nullText">Which text to use if <see cref="text"/> is null.</param>
        /// <returns>Text in a code block or null if both <see cref="text"/> and <see cref="nullText"/> are null.</returns>
        public static string? ToCode(this string? text, string? nullText = null)
        {
            if (text == null && nullText == null)
                return null;
            return $"`{text ?? nullText}`";
        }
    }
}