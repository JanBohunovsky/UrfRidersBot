namespace UrfRidersBot.Core.Common.Configuration
{
    public class DiscordOptions
    {
        public const string SectionName = "Discord";
        
        public string Prefix { get; set; } = "!";
        public string? Token { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
    }
}