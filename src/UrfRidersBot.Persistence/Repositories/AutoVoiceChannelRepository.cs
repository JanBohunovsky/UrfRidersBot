using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Persistence.Repositories
{
    public class AutoVoiceChannelRepository : IAutoVoiceChannelRepository
    {
        private readonly UrfRidersDbContext _context;

        public AutoVoiceChannelRepository(UrfRidersDbContext context)
        {
            _context = context;
        }

        public async ValueTask<IEnumerable<DiscordChannel?>> GetAllAsync(DiscordClient client)
        {
            var channels = await _context.AutoVoiceChannels.ToListAsync();
            return channels.Select(x => x.FromDTO(client));
        }

        public async ValueTask<IEnumerable<DiscordChannel?>> GetChannelsAsync(DiscordGuild guild)
        {
            //return mapper<T>.ToDiscord(channels);
            
            // TODO: The list may contain nulls (if the specified channel is not found)
            return await _context.AutoVoiceChannels
                .Where(x => x.GuildId == guild.Id)
                .Select(x => x.FromDTO(guild))
                .ToListAsync();
        }

        public async Task AddAsync(DiscordChannel channel)
        {
            await _context.AutoVoiceChannels.AddAsync(channel.ToDTO());
        }

        public void Remove(DiscordChannel channel)
        {
            _context.Remove(channel.ToDTO());
        }
    }
}