using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace UrfRidersBot.Core.Interfaces
{
    public interface IAutoVoiceService
    {
        ValueTask<DiscordChannel> EnableForGuildAsync(DiscordGuild guild, string channelName, DiscordChannel? category = null);
        ValueTask<int> DisableForGuildAsync(DiscordGuild guild);
        ValueTask<IEnumerable<DiscordChannel>> GetGuildVoiceChannels(DiscordGuild guild);

        Task CreateVoiceChannelAsync(DiscordGuild guild, string name);
        Task DeleteVoiceChannelAsync(DiscordChannel channel);
        Task UpdateVoiceChannelNameAsync(DiscordChannel channel, string name);
        ValueTask<DiscordChannel?> FindVoiceChannelAsync(DiscordClient client, DiscordUser user);
    }
}