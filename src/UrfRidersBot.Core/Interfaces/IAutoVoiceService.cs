using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace UrfRidersBot.Core.Interfaces
{
    public interface IAutoVoiceService
    {
        ValueTask<DiscordChannel> EnableForGuildAsync(DiscordGuild guild, DiscordChannel? category = null);
        ValueTask<int> DisableForGuildAsync(DiscordGuild guild);
        ValueTask<IEnumerable<DiscordChannel>> GetGuildVoiceChannels(DiscordGuild guild);

        ValueTask<DiscordChannel> CreateVoiceChannelAsync(DiscordGuild guild, DiscordChannel? category = null);
        Task DeleteVoiceChannelAsync(DiscordChannel channel);
        Task UpdateVoiceChannelNameAsync(DiscordChannel channel, string name);
        ValueTask<DiscordChannel> FindVoiceChannelAsync(DiscordUser user);
    }
}