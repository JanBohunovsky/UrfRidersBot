using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using UrfRidersBot.Core.Entities;
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

        public async ValueTask<ILookup<ulong, AutoVoiceChannel>> GetAllRawAsync()
        {
            var result = await _context.AutoVoiceChannels
                .AsNoTracking()
                .ToListAsync();

            return result.ToLookup(x => x.GuildId, x => x);
        }

        public async ValueTask<ILookup<DiscordGuild, DiscordChannel>> GetAllAsync(DiscordClient client)
        {
            var rawResult = await _context.AutoVoiceChannels
                .AsNoTracking()
                .ToListAsync();

            return rawResult
                .ToLookup(x => client.Guilds[x.GuildId],
                    x => x.ToDiscord(client));
        }

        public async ValueTask<ICollection<DiscordChannel>> GetChannelsAsync(DiscordGuild guild)
        {
            var rawResult = await _context.AutoVoiceChannels
                .AsNoTracking()
                .Where(x => x.GuildId == guild.Id)
                .ToListAsync();

            return rawResult
                .Select(x => x.ToDiscord(guild))
                .Where(x => x != null)
                .ToList();
        }

        public async ValueTask<DiscordChannel> GetVoiceChannelCreator(DiscordGuild guild)
        {
            var all = await _context.AutoVoiceChannels
                .AsNoTracking()
                .Where(x => x.GuildId == guild.Id)
                .ToListAsync();

            return all.Last().ToDiscord(guild);
        }

        public async ValueTask<bool> ContainsChannel(DiscordChannel voiceChannel)
        {
            // I don't know how EF Core compares objects in SQL queries when using Contains() method
            // and I'm too lazy to find out, so I'm just gonna use Any()  :^)
            return await _context.AutoVoiceChannels
                .AnyAsync(x => x.GuildId == voiceChannel.GuildId && x.VoiceChannelId == voiceChannel.Id);
        }

        public async Task AddChannelAsync(DiscordChannel voiceChannel)
        {
            await _context.AutoVoiceChannels.AddAsync(AutoVoiceChannel.FromDiscord(voiceChannel));
        }

        public void RemoveChannel(DiscordChannel voiceChannel)
        {
            _context.AutoVoiceChannels.Remove(AutoVoiceChannel.FromDiscord(voiceChannel));
        }

        public void RemoveChannel(AutoVoiceChannel autoVoiceChannel)
        {
            _context.AutoVoiceChannels.Remove(autoVoiceChannel);
        }
    }
}