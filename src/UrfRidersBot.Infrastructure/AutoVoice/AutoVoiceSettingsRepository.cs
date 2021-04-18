using System.Linq;
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

        public async Task CleanupAsync()
        {
            var collection = Collection;
            var dto = await collection.FindOneAsync(s => s.GuildId == _guild.Id);

            if (dto?.ChannelCreatorId is null)
            {
                return;
            }

            var channelCreator = _guild.GetChannel(dto.ChannelCreatorId.Value);

            // Channel creator was deleted -> remove all voice channels
            if (channelCreator is null)
            {
                dto.ChannelCreatorId = null;
                dto.VoiceChannels.Clear();
                
                await collection.UpsertAsync(dto);
                return;
            }

            // Remove deleted voice channels
            dto.VoiceChannels = dto.VoiceChannels
                .Where(voiceChannelId => _guild.GetChannel(voiceChannelId) is not null)
                .ToList();

            await collection.UpsertAsync(dto);
        }
    }
}