using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace UrfRidersBot.Core.Commands
{
    public interface ICommandContext
    {
        DiscordClient Client { get; }
        DiscordInteraction Interaction { get; }
        bool IsEphemeral { get; }
        
        DiscordChannel Channel { get; }
        DiscordUser User { get; }
        DiscordGuild Guild { get; }
        DiscordMember Member { get; }
        
        ValueTask<DiscordMessage> RespondAsync(DiscordFollowupMessageBuilder? builder = null);
        ValueTask<DiscordMessage> RespondAsync(string content);
        ValueTask<DiscordMessage> RespondAsync(DiscordEmbed embed);
        ValueTask<DiscordMessage> RespondAsync(string content, DiscordEmbed embed);
    }
}