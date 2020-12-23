using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace UrfRidersBot.Library
{
    public static class DbSetExtensions
    {
        public static async ValueTask<GuildData> FindOrCreateAsync(this DbSet<GuildData> guildData, ulong guildId)
        {
            var result = await guildData.FindAsync(guildId);
            if (result == null)
            {
                result = new GuildData(guildId);
                await guildData.AddAsync(result);
            }

            return result;
        }
    }
}