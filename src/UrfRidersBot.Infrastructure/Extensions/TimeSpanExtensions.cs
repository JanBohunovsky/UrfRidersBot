using System;
using System.Text;

namespace UrfRidersBot.Infrastructure
{
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Returns a more human friendly TimeSpan string.
        /// <para>
        /// Example output: 1d 8h 30m 15s
        /// </para>
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static string ToPrettyString(this TimeSpan timeSpan)
        {
            var sb = new StringBuilder();

            if (timeSpan.Days > 0)
            {
                sb.Append($"{timeSpan.Days}d ");
            }

            if (timeSpan.Hours > 0)
            {
                sb.Append($"{timeSpan.Hours}h ");
            }

            if (timeSpan.Minutes > 0)
            {
                sb.Append($"{timeSpan.Minutes}m ");
            }

            sb.Append($"{timeSpan.Seconds}s");

            return sb.ToString();
        }
    }
}