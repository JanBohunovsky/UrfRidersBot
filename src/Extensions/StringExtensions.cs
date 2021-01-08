namespace UrfRidersBot
{
    public static class StringExtensions
    {
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