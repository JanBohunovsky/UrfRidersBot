using System.Threading.Tasks;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Core.AutoVoice
{
    public interface IAutoVoiceSettingsRepository : IRepository
    {
        /// <summary>
        /// Gets AutoVoice settings for configured guild. Returns null if not found.
        /// </summary>
        /// <returns></returns>
        ValueTask<AutoVoiceSettings?> GetAsync();
        
        /// <summary>
        /// Gets AutoVoice settings for configured guild. Returns new instance if not found.
        /// </summary>
        /// <returns></returns>
        ValueTask<AutoVoiceSettings> GetOrCreateAsync();
        
        /// <summary>
        /// Saves the AutoVoice settings for configured guild.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        Task SaveAsync(AutoVoiceSettings settings);
        
        /// <summary>
        /// Removes AutoVoice settings for configured guild.
        /// </summary>
        /// <returns></returns>
        Task RemoveAsync();

        /// <summary>
        /// Removes non-existing voice channels from database for configured guild.
        /// </summary>
        /// <returns></returns>
        Task CleanupAsync();
    }
}