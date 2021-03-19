using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;

namespace UrfRidersBot.Commands.Parsers
{
    // Inspired by:
    // https://github.com/DSharpPlus/DSharpPlus/blob/master/DSharpPlus.CommandsNext/Converters/
    // https://github.com/Quahu/Disqord/blob/master/src/Disqord.Bot/Commands/Parsers/
    public class DiscordMessageTypeParser : TypeParser<DiscordMessage>
    {
        private readonly Regex _messagePathRegex;
        
        public DiscordMessageTypeParser()
        {
            _messagePathRegex = new Regex(@"^\/channels\/(?<guild>(?:\d+|@me))\/(?<channel>\d+)\/(?<message>\d+)\/?$",
                RegexOptions.ECMAScript | RegexOptions.Compiled);
        }
        
        public override async ValueTask<TypeParserResult<DiscordMessage>> ParseAsync(Parameter parameter, string value, CommandContext _)
        {
            var context = (UrfRidersCommandContext)_;
            
            if (string.IsNullOrWhiteSpace(value))
                return TypeParserResult<DiscordMessage>.Unsuccessful("Input for message cannot be empty.");

            // Try message link
            var messageUri = value.StartsWith('<') && value.EndsWith('>')
                ? value.Substring(1, value.Length - 2)
                : value;
            ulong messageId;
            
            if (Uri.TryCreate(messageUri, UriKind.Absolute, out var uri))
            {
                if (uri.Host != "discordapp.com" 
                    && uri.Host != "discord.com" 
                    && !uri.Host.EndsWith(".discordapp.com") 
                    && !uri.Host.EndsWith(".discord.com"))
                {
                    return TypeParserResult<DiscordMessage>.Unsuccessful("Invalid message link host.");
                }

                var match = _messagePathRegex.Match(uri.AbsolutePath);
                if (!match.Success
                    || !ulong.TryParse(match.Groups["channel"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var channelId)
                    || !ulong.TryParse(match.Groups["message"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out messageId))
                {
                    return TypeParserResult<DiscordMessage>.Unsuccessful("Invalid message link format.");
                }

                var channel = await context.Client.GetChannelAsync(channelId);
                if (channel is null)
                {
                    return TypeParserResult<DiscordMessage>.Unsuccessful("Message channel not found.");
                }

                var message = await channel.GetMessageAsync(messageId);
                return message is null
                    ? TypeParserResult<DiscordMessage>.Unsuccessful("Message not found.")
                    : TypeParserResult<DiscordMessage>.Successful(message);
            }
            
            // Try ID in current channel
            if (ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out messageId))
            {
                var result = await context.Channel.GetMessageAsync(messageId);
                return result is null
                    ? TypeParserResult<DiscordMessage>.Unsuccessful("Message not found.")
                    : TypeParserResult<DiscordMessage>.Successful(result);
            }

            return TypeParserResult<DiscordMessage>.Unsuccessful(
                "Invalid input format. Use message URL or, if the message is in this channel, use the message ID.");
        }
    }
}