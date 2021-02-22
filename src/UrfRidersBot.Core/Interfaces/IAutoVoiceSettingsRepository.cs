using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Entities;

namespace UrfRidersBot.Core.Interfaces
{
    public interface IAutoVoiceSettingsRepository : IRepository
    {
        ValueTask<IEnumerable<AutoVoiceSettings>> GetEnabledAsync();
        ValueTask<AutoVoiceSettings> GetByGuildAsync(DiscordGuild guild);
    }
}