using Microsoft.Extensions.Configuration;

namespace UrfRidersBot.Discord.Configuration
{
    public class RiotGamesConfiguration : BaseConfiguration
    {
        public string? ApiKey { get; set; }
        
        public RiotGamesConfiguration(IConfiguration configuration) : base(configuration, "RiotGames")
        {
        }
    }
}