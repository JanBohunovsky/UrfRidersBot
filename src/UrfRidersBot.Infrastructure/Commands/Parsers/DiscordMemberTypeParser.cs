using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;

namespace UrfRidersBot.Infrastructure.Commands.Parsers
{
    // Inspired by:
    // https://github.com/DSharpPlus/DSharpPlus/blob/master/DSharpPlus.CommandsNext/Converters/
    // https://github.com/Quahu/Disqord/blob/master/src/Disqord.Bot/Commands/Parsers/
    public class DiscordMemberTypeParser : TypeParser<DiscordMember>
    {
        private const string NotFound = "Member not found.";
        private readonly Regex _userRegex;
        
        public DiscordMemberTypeParser()
        {
            _userRegex = new Regex(@"^<@\!?(\d+)>$", RegexOptions.ECMAScript | RegexOptions.Compiled);
        }
        
        public override async ValueTask<TypeParserResult<DiscordMember>> ParseAsync(Parameter parameter, string value, CommandContext _)
        {
            var context = (UrfRidersCommandContext)_;

            // Try ID
            if (ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var userId))
            {
                var result = await context.Guild.GetMemberAsync(userId);
                return result is null
                    ? TypeParserResult<DiscordMember>.Unsuccessful("No member found with this ID.")
                    : TypeParserResult<DiscordMember>.Successful(result);
            }
            
            // Try mention
            var match = _userRegex.Match(value);
            if (match.Success && ulong.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out userId))
            {
                var result = await context.Guild.GetMemberAsync(userId);
                return result is null
                    ? TypeParserResult<DiscordMember>.Unsuccessful(NotFound)
                    : TypeParserResult<DiscordMember>.Successful(result);
            }
            
            // Try username/nickname + discriminator
            value = value.ToLowerInvariant();
            var index = value.IndexOf('#');
            var username = index != -1 ? value.Substring(0, index) : value;
            var discriminator = index != -1 ? value.Substring(index + 1) : null;

            var users = context.Guild.Members.Values
                .Where(m => 
                    m.Username.ToLowerInvariant() == username && ((discriminator is not null && m.Discriminator == discriminator) || discriminator is null)
                    || m.Nickname?.ToLowerInvariant() == value);

            var member = users.FirstOrDefault();
            return member is null
                ? TypeParserResult<DiscordMember>.Unsuccessful(NotFound)
                : TypeParserResult<DiscordMember>.Successful(member);
        }
    }
}