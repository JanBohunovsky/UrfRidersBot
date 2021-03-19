using System.Text;

namespace UrfRidersBot.Core
{
    public static class Markdown
    {
        /// <summary>
        ///     The set containing the escaped markdown characters.
        /// </summary>
        private const string EscapedCharacters = "\\*`_~";

        public static string Italics(string text) => $"*{text}*";

        public static string Bold(string text) => $"**{text}**";

        public static string BoldItalics(string text) => $"***{text}***";

        public static string Underline(string text) => $"__{text}__";

        public static string Strikethrough(string text) => $"~~{text}~~";

        public static string Link(string title, string url) => $"[{title}]({url})";

        public static string Code(string code) => $"`{code}`";

        public static string CodeBlock(string code) => $"```\n{code}```";

        public static string CodeBlock(string language, string code) => $"```{language}\n{code}```";
     
        public static string Escape(string text)
        {
            var builder = new StringBuilder(text.Length);
            foreach (var character in text)
            {
                if (EscapedCharacters.Contains(character))
                    builder.Append('\\');

                builder.Append(character);
            }

            return builder.ToString();
        }
    }
}
