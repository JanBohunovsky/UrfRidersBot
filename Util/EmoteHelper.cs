using Discord;

namespace UrfRiders.Util
{
    public static class EmoteHelper
    {
        public static IEmote Parse(string value)
        {
            IEmote result = new Emoji(value);
            if (Emote.TryParse(value, out var emote))
                result = emote;

            return result;
        }
    }
}