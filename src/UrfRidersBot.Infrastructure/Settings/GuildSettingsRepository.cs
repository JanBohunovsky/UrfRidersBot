using DSharpPlus.Entities;
using LiteDB;
using UrfRidersBot.Core.Settings;
using UrfRidersBot.Infrastructure.Common;

namespace UrfRidersBot.Infrastructure.Settings
{
    internal class GuildSettingsRepository : LiteRepository<GuildSettingsDTO>, IGuildSettingsRepository
    {
        private readonly DiscordGuild _guild;

        public GuildSettingsRepository(LiteDatabase db, DiscordGuild guild) : base(db, "guild_settings")
        {
            _guild = guild;
        }

        public GuildSettings? Get()
        {
            var result = Collection.FindOne(s => s.GuildId == _guild.Id);

            return result?.ToDiscord(_guild);
        }

        public GuildSettings GetOrCreate()
        {
            var result = Collection.FindOne(s => s.GuildId == _guild.Id)
                         ?? new GuildSettingsDTO();

            return result.ToDiscord(_guild);
        }

        public void Save(GuildSettings settings)
        {
            var dto = GuildSettingsDTO.FromDiscord(_guild.Id, settings);
            Collection.Upsert(dto);
        }

        public void Remove()
        {
            Collection.DeleteMany(s => s.GuildId == _guild.Id);
        }
    }
}