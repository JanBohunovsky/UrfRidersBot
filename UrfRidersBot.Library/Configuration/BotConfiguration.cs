using System.Globalization;
using Discord;
using Microsoft.Extensions.Configuration;

namespace UrfRidersBot.Library
{
    public class BotConfiguration
    {
        public string Name { get; set; } = "UrfRiders Bot";
        public string Prefix { get; set; } = "!";
        public Color EmbedColor { get; set; }
        public EmoteConfiguration Emotes { get; set; }

        public BotConfiguration(IConfiguration configuration, EmoteConfiguration emotes)
        {
            // Bind basic properties
            Emotes = emotes;
            configuration.Bind("Bot", this);
            
            // Manual binding for advanced properties
            var bot = configuration.GetSection("Bot");
            
            var embedColorString = bot[nameof(EmbedColor)] ?? "#1abc9c";
            if (uint.TryParse(embedColorString.Replace("#", ""), NumberStyles.HexNumber, null, out var rawValue))
            {
                EmbedColor = new Color(rawValue);
            }
        }
    }
}