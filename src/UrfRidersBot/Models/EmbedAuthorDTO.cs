using DSharpPlus.Entities;

namespace UrfRidersBot.Models
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

        public static EmbedAuthorDTO? FromDiscord(DiscordEmbedAuthor? author)
        {
            if (author == null)
                return null;

            return new EmbedAuthorDTO
            {
                Name = author.Name,
                Url = author.Url?.ToString(),
                IconUrl = author.IconUrl?.ToString()
            };
        }
    }
}