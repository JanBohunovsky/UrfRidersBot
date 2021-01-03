using System.Globalization;
using Discord;
using Microsoft.Extensions.Configuration;

namespace UrfRidersBot
{
    public class BotConfiguration : BaseConfiguration
    {
        public string Name { get; set; } = "UrfRiders Bot";
        public string Prefix { get; set; } = "!";
        public Color Color { get; }
        public EmoteConfiguration Emotes { get; }

        public BotConfiguration(IConfiguration configuration, EmoteConfiguration emotes) : base(configuration, "Bot")
        {
            var bot = configuration.GetSection("Bot");
            
            Emotes = emotes;
            var embedColorString = bot[nameof(Color)] ?? "#1abc9c";
            if (uint.TryParse(embedColorString.Replace("#", ""), NumberStyles.HexNumber, null, out var rawValue))
            {
                Color = new Color(rawValue);
            }
        }
    }
}