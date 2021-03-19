using System.Linq;
using DSharpPlus.Entities;

namespace UrfRidersBot.Models
{
    public class MessageDTO
    {
        public string? Content { get; set; }
        public EmbedDTO? Embed { get; set; }

        public static MessageDTO FromDiscord(DiscordMessage message)
        {
            return new()
            {
                Content = message.Content,
                Embed = EmbedDTO.FromDiscord(message.Embeds.FirstOrDefault())
            };
        }
    }
}