using DSharpPlus.Entities;
using LiteDB;
using UrfRidersBot.Core.AutoVoice;
using UrfRidersBot.Infrastructure.Common;

namespace UrfRidersBot.Infrastructure.AutoVoice
{
    internal class AutoVoiceSettingsRepository : LiteRepository<AutoVoiceSettingsDTO>, IAutoVoiceSettingsRepository
    {
        private readonly DiscordGuild _guild;

        public AutoVoiceSettingsRepository(LiteDatabase db, DiscordGuild guild) : base(db, "auto_voice_settings")
        {
            _guild = guild;
        }

        public AutoVoiceSettings? Get()
        {
            var result = Collection.FindOne(s => s.GuildId == _guild.Id);

            return result?.ToDiscord(_guild);
        }

        public AutoVoiceSettings GetOrCreate()
        {
            var result = Collection.FindOne(s => s.GuildId == _guild.Id)
                         ?? new AutoVoiceSettingsDTO();

            return result.ToDiscord(_guild);
        }

        public void Save(AutoVoiceSettings settings)
        {
            var dto = AutoVoiceSettingsDTO.FromDiscord(_guild.Id, settings);
            Collection.Upsert(dto);
        }

        public void Remove()
        {
            Collection.DeleteMany(s => s.GuildId == _guild.Id);
        }
    }
}