using DSharpPlus.Entities;

namespace UrfRidersBot.WebAPI.Models
{
    public class EmbedThumbnailDTO
    {
        public string? Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        
        public DiscordEmbedBuilder.EmbedThumbnail ToDiscord()
        {
            return new()
            {
                Url = Url,
                Width = Width,
                Height = Height
            };
        }
    }
}