using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace UrfRidersBot.Core.AutoVoice
{
    public interface IAutoVoiceSettingsRepository
    {
        Task AddAsync(AutoVoiceSettings settings);
        ValueTask<IEnumerable<AutoVoiceSettings>> GetEnabledAsync();
        ValueTask<AutoVoiceSettings?> GetAsync(DiscordGuild guild);
        ValueTask<AutoVoiceSettings> GetOrCreateAsync(DiscordGuild guild);
    }
}