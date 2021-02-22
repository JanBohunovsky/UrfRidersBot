using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Entities;

namespace UrfRidersBot.Core.Interfaces
{
    public interface IAutoVoiceChannelRepository
    {
        ValueTask<ILookup<ulong, AutoVoiceChannel>> GetAllRawAsync();
        ValueTask<ILookup<DiscordGuild, DiscordChannel>> GetAllAsync(DiscordClient client);
        ValueTask<ICollection<DiscordChannel>> GetChannelsAsync(DiscordGuild guild);
        ValueTask<DiscordChannel> GetVoiceChannelCreator(DiscordGuild guild);
        ValueTask<bool> ContainsChannel(DiscordChannel voiceChannel);
        Task AddChannelAsync(DiscordChannel voiceChannel);
        void RemoveChannel(DiscordChannel voiceChannel);
        void RemoveChannel(AutoVoiceChannel autoVoiceChannel);
    }
}