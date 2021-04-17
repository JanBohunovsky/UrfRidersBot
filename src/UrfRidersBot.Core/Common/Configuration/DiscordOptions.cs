namespace UrfRidersBot.Core.Common.Configuration
{
    public class DiscordOptions
    {
        public const string SectionName = "Discord";
        
        public string? Token { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public ulong GuildId { get; set; }
    }
}