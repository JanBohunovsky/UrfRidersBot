using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace UrfRidersBot.Library
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
    }
}