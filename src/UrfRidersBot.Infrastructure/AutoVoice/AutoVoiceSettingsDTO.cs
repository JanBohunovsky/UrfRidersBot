using System.Collections.Generic;
using System.Linq;
using DSharpPlus.Entities;
using LiteDB;
using UrfRidersBot.Core.AutoVoice;

namespace UrfRidersBot.Infrastructure.AutoVoice
{
    internal class AutoVoiceSettingsDTO
    {
        [BsonId]
        public ulong GuildId { get; set; }
        public int? Bitrate { get; set; }
        public ulong? ChannelCreatorId { get; set; }
        public List<ulong> VoiceChannels { get; set; } = new();

        public AutoVoiceSettings ToDiscord(DiscordGuild guild)
        {
            var voiceChannels = VoiceChannels
                .Select(guild.GetChannel)
                .Where(x => x is not null);
            
            var channelCreator = ChannelCreatorId is null
                ? null
                : guild.GetChannel(ChannelCreatorId.Value);

            return new AutoVoiceSettings(voiceChannels)
            {
                Bitrate = Bitrate,
                ChannelCreator = channelCreator
            };
        }

        public static AutoVoiceSettingsDTO FromDiscord(ulong guildId, AutoVoiceSettings settings)
        {
            return new()
            {
                GuildId = guildId,
                Bitrate = settings.Bitrate,
                ChannelCreatorId = settings.ChannelCreator?.Id,
                VoiceChannels = settings.VoiceChannels.Select(c => c.Id).ToList()
            };
        }
    }
}