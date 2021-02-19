using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace UrfRidersBot.Core.Interfaces
{
    public interface IAutoVoiceChannelRepository : IRepository
    {
        ValueTask<IEnumerable<DiscordChannel?>> GetAllAsync(DiscordClient client);
        ValueTask<IEnumerable<DiscordChannel?>> GetChannelsAsync(DiscordGuild guild);
        Task AddAsync(DiscordChannel channel);
        void Remove(DiscordChannel channel);
    }
}