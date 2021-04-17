using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Core.Settings
{
    public interface IGuildSettingsRepository : IRepository
    {
        GuildSettings? Get();
        GuildSettings GetOrCreate();
        void Save(GuildSettings settings);
        void Remove();
    }
}