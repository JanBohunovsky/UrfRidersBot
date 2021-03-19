using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;

namespace UrfRidersBot.Infrastructure.Commands.Parsers
{
    // Inspired by:
    // https://github.com/DSharpPlus/DSharpPlus/blob/master/DSharpPlus.CommandsNext/Converters/
    // https://github.com/Quahu/Disqord/blob/master/src/Disqord.Bot/Commands/Parsers/
    public class DiscordUserTypeParser : TypeParser<DiscordUser>
    {
        private DiscordMemberTypeParser? _memberTypeParser;
        
        public override async ValueTask<TypeParserResult<DiscordUser>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            _memberTypeParser ??= parameter.Service.GetSpecificTypeParser<DiscordMember, DiscordMemberTypeParser>()
                                  ?? new DiscordMemberTypeParser();

            var result = await _memberTypeParser.ParseAsync(parameter, value, context);
            return result.IsSuccessful
                ? TypeParserResult<DiscordUser>.Successful(result.Value)
                : TypeParserResult<DiscordUser>.Unsuccessful(result.Reason);
        }
    }
}