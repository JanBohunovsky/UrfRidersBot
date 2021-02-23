using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Entities;

namespace UrfRidersBot.Core.Interfaces
{
    public interface IGuildSettingsRepository
    {
        ValueTask<GuildSettings?> GetAsync(DiscordGuild guild);
        ValueTask<GuildSettings> GetOrCreateAsync(DiscordGuild guild);
        void Remove(GuildSettings guildSettings);
    }
}