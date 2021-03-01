using System;
using System.Collections.Generic;
using DSharpPlus.Entities;

namespace UrfRidersBot.WebAPI.Models
{
    public class EmbedDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
        public string? ImageUrl { get; set; }
        public string? Color { get; set; }
        public EmbedAuthorDTO? Author { get; set; }
        public EmbedThumbnailDTO? Thumbnail { get; set; }
        public EmbedFooterDTO? Footer { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public IEnumerable<EmbedFieldDTO> Fields { get; set; } = new List<EmbedFieldDTO>();

        public DiscordEmbedBuilder ToDiscord()
        {
            var builder = new DiscordEmbedBuilder
            {
                Title = Title,
                Description = Description,
                Url = Url,
                ImageUrl = ImageUrl,
                Color = Color != null ? new DiscordColor(Color) : new Optional<DiscordColor>(),
                Author = Author?.ToDiscord(),
                Thumbnail = Thumbnail?.ToDiscord(),
                Footer = Footer?.ToDiscord(),
                Timestamp = Timestamp
            };

            foreach (var field in Fields)
            {
                builder.AddField(field.Name, field.Value, field.Inline);
            }

            return builder;
        }
    }
}