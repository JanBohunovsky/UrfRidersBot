using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;

namespace UrfRidersBot.Infrastructure.Commands.Parsers
{
    // Inspired by:
    // https://github.com/DSharpPlus/DSharpPlus/blob/master/DSharpPlus.CommandsNext/Converters/
    // https://github.com/Quahu/Disqord/blob/master/src/Disqord.Bot/Commands/Parsers/
    public class DiscordGuildTypeParser : TypeParser<DiscordGuild>
    {
        public override ValueTask<TypeParserResult<DiscordGuild>> ParseAsync(Parameter parameter, string value, CommandContext _)
        {
            var context = (UrfRidersCommandContext)_;
            
            // Try ID
            if (ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var guildId))
            {
                if (context.Client.Guilds.TryGetValue(guildId, out var result))
                {
                    return new ValueTask<TypeParserResult<DiscordGuild>>(
                        TypeParserResult<DiscordGuild>.Successful(result));
                }
                else
                {
                    return new ValueTask<TypeParserResult<DiscordGuild>>(
                        TypeParserResult<DiscordGuild>.Unsuccessful("No guild found with this ID."));
                }
            }
            
            // Try name
            value = value.ToLowerInvariant();

            var guild = context.Client.Guilds.Values.FirstOrDefault(g => g.Name.ToLowerInvariant() == value);
            return new ValueTask<TypeParserResult<DiscordGuild>>(
                guild is null
                    ? TypeParserResult<DiscordGuild>.Unsuccessful("Guild not found.")
                    : TypeParserResult<DiscordGuild>.Successful(guild));
        }
    }
}