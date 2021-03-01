using DSharpPlus.Entities;

namespace UrfRidersBot.WebAPI.Models
{
    public class EmbedAuthorDTO
    {
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? IconUrl { get; set; }
        
        public DiscordEmbedBuilder.EmbedAuthor ToDiscord()
        {
            return new()
            {
                Name = Name,
                Url = Url,
                IconUrl = IconUrl
            };
        }
    }
}