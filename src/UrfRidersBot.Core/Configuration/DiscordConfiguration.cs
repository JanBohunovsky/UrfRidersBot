using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Core.Configuration
{
    public class DiscordConfiguration : IApplicationConfiguration
    {
        public const string SectionName = "Discord";
        
        public string Prefix { get; set; } = "!";
        public string? Token { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
    }
}