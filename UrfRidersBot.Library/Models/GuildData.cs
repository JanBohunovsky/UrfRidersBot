using System.ComponentModel.DataAnnotations;

namespace UrfRidersBot.Library
{
    public class GuildData
    {
        [Key]
        public ulong GuildId { get; set; }
        
        public string? RandomValue { get; set; }

        public GuildData(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}