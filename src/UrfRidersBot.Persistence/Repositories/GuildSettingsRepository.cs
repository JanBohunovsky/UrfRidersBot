using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Entities;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Persistence.Repositories
{
    public class GuildSettingsRepository : IGuildSettingsRepository
    {
        private readonly UrfRidersDbContext _context;

        public GuildSettingsRepository(UrfRidersDbContext context)
        {
            _context = context;
        }
        
        public async ValueTask<GuildSettings?> GetAsync(DiscordGuild guild)
        {
            return await _context.GuildSettings.FindAsync(guild.Id);
        }

        public async ValueTask<GuildSettings> GetOrCreateAsync(DiscordGuild guild)
        {
            var result = await _context.GuildSettings.FindAsync(guild.Id);
            if (result == null)
            {
                result = new GuildSettings(guild.Id);
                await _context.GuildSettings.AddAsync(result);
            }

            return result;
        }

        public void Remove(GuildSettings guildSettings)
        {
            _context.GuildSettings.Remove(guildSettings);
        }
    }
}