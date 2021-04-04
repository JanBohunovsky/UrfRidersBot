using Newtonsoft.Json;
using UrfRidersBot.Core.Commands.Models;

namespace UrfRidersBot.Infrastructure.Commands.Models
{
    internal class InteractionCreatePayload
    {
        [JsonProperty("type")]
        public DiscordInteractionResponseType Type { get; set; }
        
        [JsonProperty("data")]
        public DiscordInteractionResponseBuilder? Data { get; set; }
    }
}