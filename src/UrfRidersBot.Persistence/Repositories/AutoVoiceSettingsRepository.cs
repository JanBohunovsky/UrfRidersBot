using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using UrfRidersBot.Core.Entities;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Persistence.Repositories
{
    public class AutoVoiceSettingsRepository : IAutoVoiceSettingsRepository
    {
        private readonly UrfRidersDbContext _context;

        public AutoVoiceSettingsRepository(UrfRidersDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AutoVoiceSettings settings)
        {
            await _context.AutoVoiceSettings.AddAsync(settings);
        }

        public async ValueTask<IEnumerable<AutoVoiceSettings>> GetEnabledAsync()
        {
            return await _context.AutoVoiceSettings
                .Where(s => s.ChannelCreatorId != null)
                .Include(s => s.VoiceChannels)
                .ToListAsync();
        }

        public async ValueTask<AutoVoiceSettings?> GetAsync(DiscordGuild guild)
        {
            return await _context.AutoVoiceSettings
                .Include(s => s.VoiceChannels)
                .SingleOrDefaultAsync(s => s.GuildId == guild.Id);
        }

        public async ValueTask<AutoVoiceSettings> GetOrCreateAsync(DiscordGuild guild)
        {
            var result = await GetAsync(guild);
            if (result is null)
            {
                result = new AutoVoiceSettings(guild.Id);
                await _context.AutoVoiceSettings.AddAsync(result);
            }

            return result;
        }
    }
}