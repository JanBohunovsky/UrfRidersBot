using Microsoft.Extensions.Configuration;

namespace UrfRidersBot
{
    public class BotConfiguration : BaseConfiguration
    {
        public string Name { get; set; } = "UrfRiders Bot";
        public string Prefix { get; set; } = "!";

        public BotConfiguration(IConfiguration configuration) : base(configuration, "Bot")
        {
        }
    }
}