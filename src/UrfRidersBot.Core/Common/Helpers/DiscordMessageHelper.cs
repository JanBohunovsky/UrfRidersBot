using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace UrfRidersBot.Core.Common
{
    public static class DiscordMessageHelper
    {
        private static readonly Regex MessageLinkRegex;
        
        static DiscordMessageHelper()
        {
            // This regex matches the message link that you can get by choosing "Copy Message Link"
            // option in the context menu of the message in Discord.
            // This does not match message links in DMs (they have '@me' instead of guild ID).
            // Example match: https://discord.com/channels/158536618750377985/815984885235843133/815985377957380127
            MessageLinkRegex = new Regex(
                @"^https?:\/\/discord\.com\/channels\/(?<guild>\d+)\/(?<channel>\d+)\/(?<message>\d+)$",
                RegexOptions.Compiled
            );
        }
        
        public static async ValueTask<DiscordMessage?> FromLinkAsync(DiscordGuild guild, string link)
        {
            var match = MessageLinkRegex.Match(link);

            if (!match.Success)
            {
                return null;
            }

            if (!ulong.TryParse(match.Groups["guild"].Value, out var guildId))
            {
                return null;
            }

            if (guild.Id != guildId)
            {
                return null;
            }

            if (!ulong.TryParse(match.Groups["channel"].Value, out var channelId))
            {
                return null;
            }

            if (!ulong.TryParse(match.Groups["message"].Value, out var messageId))
            {
                return null;
            }

            var channel = guild.GetChannel(channelId);
            return await channel.GetMessageAsync(messageId);
        }
    }
}