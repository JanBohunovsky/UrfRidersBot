namespace UrfRidersBot.WebAPI.Models
{
    public class MessageDTO
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public string? Content { get; set; }
        public EmbedDTO? Embed { get; set; }
    }
}