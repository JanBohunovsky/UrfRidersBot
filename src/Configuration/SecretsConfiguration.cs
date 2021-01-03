using Microsoft.Extensions.Configuration;

namespace UrfRidersBot
{
    public class SecretsConfiguration : BaseConfiguration
    {
        public string? DiscordToken { get; set; }
        public string? RiotApiKey { get; set; }

        public SecretsConfiguration(IConfiguration configuration) : base(configuration, "Secrets")
        {
        }
    }
}