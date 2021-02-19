namespace UrfRidersBot.Persistence.DTOs
{
    public class AutoVoiceChannelDTO
    {
        public ulong GuildId { get; set; }
        public ulong VoiceChannelId { get; set; }
        
        public AutoVoiceChannelDTO(ulong guildId, ulong voiceChannelId)
        {
            GuildId = guildId;
            VoiceChannelId = voiceChannelId;
        }
    }
}