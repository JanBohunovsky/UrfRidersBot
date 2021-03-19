using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;

namespace UrfRidersBot.Commands.Parsers
{
    // Inspired by:
    // https://github.com/DSharpPlus/DSharpPlus/blob/master/DSharpPlus.CommandsNext/Converters/
    // https://github.com/Quahu/Disqord/blob/master/src/Disqord.Bot/Commands/Parsers/
    public class DiscordChannelTypeParser : TypeParser<DiscordChannel>
    {
        private const string NotFound = "Channel not found.";
        private readonly Regex _channelRegex;
        
        public DiscordChannelTypeParser()
        {
            _channelRegex = new Regex(@"^<#(\d+)>$", RegexOptions.ECMAScript | RegexOptions.Compiled);
        }
        
        public override async ValueTask<TypeParserResult<DiscordChannel>> ParseAsync(Parameter parameter, string value, CommandContext _)
        {
            var context = (UrfRidersCommandContext)_;
            
            // Try ID
            if (ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var channelId))
            {
                var result = await context.Client.GetChannelAsync(channelId);
                return result is null
                    ? TypeParserResult<DiscordChannel>.Unsuccessful("No channel found with this ID.")
                    : TypeParserResult<DiscordChannel>.Successful(result);
            }
            
            // Try mention
            var match = _channelRegex.Match(value);
            if (match.Success && ulong.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out channelId))
            {
                var result = await context.Client.GetChannelAsync(channelId);
                return result is null
                    ? TypeParserResult<DiscordChannel>.Unsuccessful(NotFound)
                    : TypeParserResult<DiscordChannel>.Successful(result);
            }
            
            // Try name
            value = value.ToLowerInvariant();

            var channel = context.Guild.Channels.Values.FirstOrDefault(c => c.Name.ToLowerInvariant() == value);
            return channel is null
                ? TypeParserResult<DiscordChannel>.Unsuccessful(NotFound)
                : TypeParserResult<DiscordChannel>.Successful(channel);
        }
    }
}