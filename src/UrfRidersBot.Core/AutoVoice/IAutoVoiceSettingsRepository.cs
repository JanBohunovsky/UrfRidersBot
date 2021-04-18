using System.Threading.Tasks;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Core.AutoVoice
{
    public interface IAutoVoiceSettingsRepository : IRepository
    {
        ValueTask<AutoVoiceSettings?> GetAsync();
        ValueTask<AutoVoiceSettings> GetOrCreateAsync();
        Task SaveAsync(AutoVoiceSettings settings);
        Task RemoveAsync();
    }
}