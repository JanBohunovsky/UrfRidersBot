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

        public static EmbedThumbnailDTO? FromDiscord(DiscordEmbedThumbnail? thumbnail)
        {
            if (thumbnail == null)
                return null;

            return new EmbedThumbnailDTO
            {
                Url = thumbnail.Url?.ToString(),
                Width = thumbnail.Width,
                Height = thumbnail.Height
            };
        }
    }
}