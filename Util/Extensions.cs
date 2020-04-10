using Discord;

namespace UrfRiders.Util
{
    public static class Extensions
    {
        public static string ToCode(this string text, bool largeCode = false) => largeCode ? $"```{text}```" : $"`{text}`";

        #region EmbedBuilder
        /// <summary>
        /// Sets embed color to green, sets the title to 'Success' if it doesn't have one, and appends '✅' to it.
        /// </summary>
        public static EmbedBuilder WithSuccess(this EmbedBuilder builder, string description = null)
        {
            builder.Title = $"{(builder.Title ?? "Success")} ✅";
            builder.WithColor(new Color(0x6ef273));
            if (!string.IsNullOrWhiteSpace(description))
                builder.WithDescription(description);
            return builder;
        }

        /// <summary>
        /// Sets embed color to yellow, sets the title to 'Error' if it doesn't have one, and appends '⚠' to it.
        /// </summary>
        public static EmbedBuilder WithError(this EmbedBuilder builder, string description = null)
        {
            builder.Title = $"{(builder.Title ?? "Error")} ⚠";
            builder.WithColor(new Color(0xfff780));
            if (!string.IsNullOrWhiteSpace(description))
                builder.WithDescription(description);
            return builder;
        }
        #endregion
    }
}