using DSharpPlus.Entities;

namespace UrfRidersBot.Core.Entities
{
    public class AutoVoiceChannel
    {
        public ulong VoiceChannelId { get; private set; }
        public ulong GuildId { get; private set; }
        
        public AutoVoiceChannel(ulong voiceChannelId, ulong guildId)
        {
            VoiceChannelId = voiceChannelId;
            GuildId = guildId;
        }

        public DiscordChannel AsDiscordChannel(DiscordGuild guild)
        {
            return guild.Channels[VoiceChannelId];
        }
    }
}