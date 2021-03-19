using DSharpPlus.Entities;

namespace UrfRidersBot.Models
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

        public static EmbedFooterDTO? FromDiscord(DiscordEmbedFooter? footer)
        {
            if (footer == null)
                return null;

            return new EmbedFooterDTO
            {
                Text = footer.Text,
                IconUrl = footer.IconUrl?.ToString()
            };
        }
    }
}