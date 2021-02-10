using Microsoft.Extensions.Configuration;

namespace UrfRidersBot.Discord.Configuration
{
    public class DiscordConfiguration : BaseConfiguration
    {
        public string Prefix { get; set; } = "!";
        public string? Token { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        
        public DiscordConfiguration(IConfiguration configuration) : base(configuration, "Discord")
        {
        }
    }
}