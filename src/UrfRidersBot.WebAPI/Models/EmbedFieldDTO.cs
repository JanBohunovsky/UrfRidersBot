using DSharpPlus.Entities;

namespace UrfRidersBot.WebAPI.Models
{
    public class EmbedFieldDTO
    {
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
        public bool Inline { get; set; }

        public static EmbedFieldDTO FromDiscord(DiscordEmbedField field)
        {
            return new()
            {
                Name = field.Name,
                Value = field.Value,
                Inline = field.Inline
            };
        }
    }
}