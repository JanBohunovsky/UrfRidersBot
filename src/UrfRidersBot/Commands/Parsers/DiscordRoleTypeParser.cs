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
    public class DiscordRoleTypeParser : TypeParser<DiscordRole>
    {
        private const string NotFound = "Role not found.";
        private readonly Regex _roleRegex;
        
        public DiscordRoleTypeParser()
        {
            _roleRegex = new Regex(@"^<@&(\d+)>$", RegexOptions.ECMAScript | RegexOptions.Compiled);
        }
        
        public override ValueTask<TypeParserResult<DiscordRole>> ParseAsync(Parameter parameter, string value, CommandContext _)
        {
            var context = (UrfRidersCommandContext)_;
            
            // Try ID
            if (ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var roleId))
            {
                var result = context.Guild.GetRole(roleId);
                return result is null
                    ? TypeParserResult<DiscordRole>.Unsuccessful("No role found with this ID.")
                    : TypeParserResult<DiscordRole>.Successful(result);
            }
            
            // Try mention
            var match = _roleRegex.Match(value);
            if (match.Success && ulong.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out roleId))
            {
                var result = context.Guild.GetRole(roleId);
                return result is null
                    ? TypeParserResult<DiscordRole>.Unsuccessful(NotFound)
                    : TypeParserResult<DiscordRole>.Successful(result);
            }
            
            // Try name
            value = value.ToLowerInvariant();

            var role = context.Guild.Roles.Values.FirstOrDefault(r => r.Name.ToLowerInvariant() == value);
            return role is null
                ? TypeParserResult<DiscordRole>.Unsuccessful(NotFound)
                : TypeParserResult<DiscordRole>.Successful(role);
        }
    }
}