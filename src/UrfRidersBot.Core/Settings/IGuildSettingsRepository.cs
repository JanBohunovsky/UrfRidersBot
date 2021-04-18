using System.Threading.Tasks;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Core.Settings
{
    public interface IGuildSettingsRepository : IRepository
    {
        ValueTask<GuildSettings?> GetAsync();
        ValueTask<GuildSettings> GetOrCreateAsync();
        Task SaveAsync(GuildSettings settings);
        Task RemoveAsync();
    }
}