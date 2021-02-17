using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Core.Configuration
{
    public class RiotGamesConfiguration : IApplicationConfiguration
    {
        public const string SectionName = "RiotGames";
        
        public string? ApiKey { get; set; }
    }
}