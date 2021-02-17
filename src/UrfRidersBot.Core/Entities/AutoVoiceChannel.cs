namespace UrfRidersBot.Core.Entities
{
    public class AutoVoiceChannel
    {
        public ulong GuildId { get; set; }
        public ulong VoiceChannelId { get; set; }
        
        public AutoVoiceChannel(ulong guildId, ulong voiceChannelId)
        {
            GuildId = guildId;
            VoiceChannelId = voiceChannelId;
        }
    }
}