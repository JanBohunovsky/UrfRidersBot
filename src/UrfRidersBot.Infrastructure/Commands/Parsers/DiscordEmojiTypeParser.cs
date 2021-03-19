using System.Collections.Generic;
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
    public class DiscordEmojiTypeParser : TypeParser<DiscordEmoji>
    {
        private readonly Regex _emoteRegex;
        
        public DiscordEmojiTypeParser()
        {
            _emoteRegex = new Regex(@"^<(?<animated>a)?:(?<name>[a-zA-Z0-9_]+?):(?<id>\d+?)>$",
                RegexOptions.ECMAScript | RegexOptions.Compiled);
        }
        
        public override ValueTask<TypeParserResult<DiscordEmoji>> ParseAsync(Parameter parameter, string value, CommandContext _)
        {
            var context = (UrfRidersCommandContext)_;

            DiscordEmoji? emoji;
            
            // Try emote
            var match = _emoteRegex.Match(value);
            if (match.Success)
            {
                var rawId = match.Groups["id"].Value;
                var name = match.Groups["name"].Value;
                var animated = match.Groups["animated"].Success;

                if (!ulong.TryParse(rawId, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
                {
                    return new ValueTask<TypeParserResult<DiscordEmoji>>(
                        TypeParserResult<DiscordEmoji>.Unsuccessful("Emote not found."));
                }

                try
                {
                    emoji = DiscordEmoji.FromGuildEmote(context.Client, id);
                    return new ValueTask<TypeParserResult<DiscordEmoji>>(
                        TypeParserResult<DiscordEmoji>.Successful(emoji));
                } catch (KeyNotFoundException) 
                { }
            }
            
            // Try unicode
            emoji = DiscordEmoji.FromUnicode(context.Client, value);
            if (emoji is not null)
            {
                return new ValueTask<TypeParserResult<DiscordEmoji>>(TypeParserResult<DiscordEmoji>.Successful(emoji));
            }

            return new ValueTask<TypeParserResult<DiscordEmoji>>(
                TypeParserResult<DiscordEmoji>.Unsuccessful("Emote not found."));
        }
    }
}