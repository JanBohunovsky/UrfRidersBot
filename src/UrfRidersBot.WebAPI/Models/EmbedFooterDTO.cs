using DSharpPlus.Entities;

namespace UrfRidersBot.WebAPI.Models
{
    public class EmbedFooterDTO
    {
        public string? Text { get; set; }
        public string? IconUrl { get; set; }
        
        public DiscordEmbedBuilder.EmbedFooter ToDiscord()
        {
            return new()
            {
                Text = Text,
                IconUrl = IconUrl
            };
        }
    }
}