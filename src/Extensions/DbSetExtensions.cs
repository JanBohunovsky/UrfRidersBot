using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace UrfRidersBot
{
    public static class DbSetExtensions
    {
        public static async ValueTask<GuildSettings> FindOrCreateAsync(this DbSet<GuildSettings> settings, ulong guildId)
        {
            var result = await settings.FindAsync(guildId);
            if (result == null)
            {
                result = new GuildSettings(guildId);
                await settings.AddAsync(result);
            }

            return result;
        }
        
        public static GuildSettings FindOrCreate(this DbSet<GuildSettings> settings, ulong guildId)
        {
            var result = settings.Find(guildId);
            if (result == null)
            {
                result = new GuildSettings(guildId);
                settings.Add(result);
            }

            return result;
        }
    }
}