namespace UrfRidersBot.Core.Entities
{
    public class GuildSettings
    {
        public ulong GuildId { get; set; }
        public string? CustomPrefix { get; set; }
        public ulong? MemberRoleId { get; set; }
        public ulong? ModeratorRoleId { get; set; }
        public ulong? AdminRoleId { get; set; }
        public int? VoiceBitrate { get; set; }

        public GuildSettings(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}