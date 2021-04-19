using System.Threading.Tasks;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Core.Settings
{
    public interface IGuildSettingsRepository : IRepository
    {
        /// <summary>
        /// Gets guild settings for configured guild. Returns null if not found.
        /// </summary>
        /// <returns></returns>
        ValueTask<GuildSettings?> GetAsync();
        
        /// <summary>
        /// Gets guild settings for configured guild. Returns new instance if not found.
        /// </summary>
        /// <returns></returns>
        ValueTask<GuildSettings> GetOrCreateAsync();
        
        /// <summary>
        /// Saves guild settings for configured guild.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        Task SaveAsync(GuildSettings settings);
        
        /// <summary>
        /// Removes guild settings for configured guild.
        /// </summary>
        /// <returns></returns>
        Task RemoveAsync();
    }
}