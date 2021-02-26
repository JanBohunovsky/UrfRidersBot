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
        ValueTask<DiscordChannel> GetCreator(DiscordGuild guild);
        ValueTask<bool> Contains(DiscordChannel voiceChannel);
        Task AddAsync(DiscordChannel voiceChannel);
        void Remove(DiscordChannel voiceChannel);
        void Remove(AutoVoiceChannel autoVoiceChannel);
    }
}