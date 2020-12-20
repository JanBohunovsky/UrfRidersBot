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
    }
}