using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace UrfRidersBot.Discord
{
    public interface IAutoVoiceService
    {
        IAsyncEnumerable<DiscordChannel> GetChannels(DiscordGuild guild);
        ValueTask<DiscordChannel> Enable(DiscordGuild guild, DiscordChannel? category = null);
        ValueTask<int> Disable(DiscordGuild guild);
    }
}