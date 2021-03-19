using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;

namespace UrfRidersBot.Infrastructure.Commands.Parsers
{
    // Inspired by:
    // https://github.com/DSharpPlus/DSharpPlus/blob/master/DSharpPlus.CommandsNext/Converters/
    // https://github.com/Quahu/Disqord/blob/master/src/Disqord.Bot/Commands/Parsers/
    public class DiscordColorTypeParser : TypeParser<DiscordColor>
    {
        private readonly Regex _colorRegexHex;
        private readonly Regex _colorRegexRgb;

        public DiscordColorTypeParser()
        {
            _colorRegexHex = new Regex(@"^#?([a-fA-F0-9]{6})$", RegexOptions.ECMAScript | RegexOptions.Compiled);
            _colorRegexRgb = new Regex(@"^(\d{1,3})\s*?,\s*?(\d{1,3}),\s*?(\d{1,3})$",
                RegexOptions.ECMAScript | RegexOptions.Compiled);
        }
        
        public override ValueTask<TypeParserResult<DiscordColor>> ParseAsync(Parameter parameter, string value, CommandContext _)
        {
            // Try hex
            var match = _colorRegexHex.Match(value);
            if (match.Success
                && int.TryParse(match.Groups[1].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                    out var color))
            {
                return new ValueTask<TypeParserResult<DiscordColor>>(
                    TypeParserResult<DiscordColor>.Successful(new DiscordColor(color)));
            }
            
            // Try RGB
            match = _colorRegexRgb.Match(value);
            if (match.Success)
            {
                var p1 = byte.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var red);
                var p2 = byte.TryParse(match.Groups[2].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var green);
                var p3 = byte.TryParse(match.Groups[3].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var blue);

                if (!(p1 && p2 && p3))
                {
                    return new ValueTask<TypeParserResult<DiscordColor>>(
                        TypeParserResult<DiscordColor>.Unsuccessful("Invalid format."));
                }

                return new ValueTask<TypeParserResult<DiscordColor>>(
                    TypeParserResult<DiscordColor>.Successful(new DiscordColor(red, green, blue)));
            }

            return new ValueTask<TypeParserResult<DiscordColor>>(
                TypeParserResult<DiscordColor>.Unsuccessful("Invalid format."));
        }
    }
}