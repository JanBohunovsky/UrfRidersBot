using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace UrfRidersBot.Core.Settings
{
    public interface IGuildSettingsRepository
    {
        ValueTask<Settings.GuildSettings?> GetAsync(DiscordGuild guild);
        ValueTask<Settings.GuildSettings> GetOrCreateAsync(DiscordGuild guild);
        void Remove(Settings.GuildSettings guildSettings);
    }
}