using Microsoft.Extensions.Configuration;

namespace UrfRidersBot.Library
{
    public class SecretsConfiguration
    {
        public string? DiscordToken { get; set; }
        public string? RiotApiKey { get; set; }

        public SecretsConfiguration(IConfiguration configuration)
        {
            configuration.Bind("Secrets", this);
        }
    }
}