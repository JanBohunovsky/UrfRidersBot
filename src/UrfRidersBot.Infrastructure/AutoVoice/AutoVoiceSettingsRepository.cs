using System.Threading.Tasks;
using DSharpPlus.Entities;
using LiteDB.Async;
using UrfRidersBot.Core.AutoVoice;
using UrfRidersBot.Infrastructure.Common;

namespace UrfRidersBot.Infrastructure.AutoVoice
{
    internal class AutoVoiceSettingsRepository : LiteRepository<AutoVoiceSettingsDTO>, IAutoVoiceSettingsRepository
    {
        private readonly DiscordGuild _guild;

        public AutoVoiceSettingsRepository(ILiteDatabaseAsync db, DiscordGuild guild) : base(db, "auto_voice_settings")
        {
            _guild = guild;
        }

        public async ValueTask<AutoVoiceSettings?> GetAsync()
        {
            var result = await Collection.FindOneAsync(s => s.GuildId == _guild.Id);

            return result?.ToDiscord(_guild);
        }

        public async ValueTask<AutoVoiceSettings> GetOrCreateAsync()
        {
            var result = await Collection.FindOneAsync(s => s.GuildId == _guild.Id) 
                         ?? new AutoVoiceSettingsDTO();

            return result.ToDiscord(_guild);
        }

        public async Task SaveAsync(AutoVoiceSettings settings)
        {
            var dto = AutoVoiceSettingsDTO.FromDiscord(_guild.Id, settings);
            await Collection.UpsertAsync(dto);
        }

        public async Task RemoveAsync()
        {
            await Collection.DeleteManyAsync(s => s.GuildId == _guild.Id);
        }
    }
}