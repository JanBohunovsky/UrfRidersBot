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

        public async ValueTask<IEnumerable<AutoVoiceSettings>> GetEnabledAsync()
        {
            return await _context.AutoVoiceSettings
                .Where(x => x.IsEnabled)
                .Include(x => x.VoiceChannels)
                .ToListAsync();
        }

        public async ValueTask<AutoVoiceSettings> GetByGuildAsync(DiscordGuild guild)
        {
            var result = await _context.AutoVoiceSettings
                .Include(x => x.VoiceChannels)
                .SingleOrDefaultAsync(x => x.GuildId == guild.Id);

            if (result == null)
            {
                result = new AutoVoiceSettings(guild.Id);
                await _context.AutoVoiceSettings.AddAsync(result);
            }

            return result;
        }
    }
}