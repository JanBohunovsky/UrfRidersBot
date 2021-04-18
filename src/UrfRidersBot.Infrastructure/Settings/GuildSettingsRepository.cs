using System.Threading.Tasks;
using DSharpPlus.Entities;
using LiteDB.Async;
using UrfRidersBot.Core.Settings;
using UrfRidersBot.Infrastructure.Common;

namespace UrfRidersBot.Infrastructure.Settings
{
    internal class GuildSettingsRepository : LiteRepository<GuildSettingsDTO>, IGuildSettingsRepository
    {
        private readonly DiscordGuild _guild;

        public GuildSettingsRepository(ILiteDatabaseAsync db, DiscordGuild guild) : base(db, "guild_settings")
        {
            _guild = guild;
        }

        public async ValueTask<GuildSettings?> GetAsync()
        {
            var result = await Collection.FindOneAsync(s => s.GuildId == _guild.Id);

            return result?.ToDiscord(_guild);
        }

        public async ValueTask<GuildSettings> GetOrCreateAsync()
        {
            var result = await Collection.FindOneAsync(s => s.GuildId == _guild.Id)
                         ?? new GuildSettingsDTO();

            return result.ToDiscord(_guild);
        }

        public async Task SaveAsync(GuildSettings settings)
        {
            var dto = GuildSettingsDTO.FromDiscord(_guild.Id, settings);
            await Collection.UpsertAsync(dto);
        }

        public async Task RemoveAsync()
        {
            await Collection.DeleteManyAsync(s => s.GuildId == _guild.Id);
        }
    }
}