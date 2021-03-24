using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DSharpPlus;
using Newtonsoft.Json;
using UrfRidersBot.Core.Commands.Entities;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Infrastructure
{
    // Inspired by:
    // https://github.com/IDoEverything/DSharpPlusSlashCommands
    public class InteractionService : IInteractionService
    {
        private readonly DiscordClient _client;
        private readonly HttpClient _http;

        public InteractionService(DiscordClient client, HttpClient http)
        {
            _client = client;
            _http = http;
        }
        
        public async Task CreateResponseAsync(
            ulong interactionId, 
            string token,
            DiscordInteractionResponseType type,
            DiscordInteractionResponseBuilder? builder = null)
        {
            var payload = new InteractionCreatePayload
            {
                Type = type,
                Data = builder
            };

            var endpoint = $"/interactions/{interactionId}/{token}/callback";
            var content = JsonContent.Create(payload);
            await _http.PostAsJsonAsync(endpoint, content);
        }

        public async Task EditResponseAsync(string token, DiscordInteractionResponseBuilder builder)
        {
            var endpoint = $"/webhooks/{_client.CurrentApplication.Id}/{token}/messages/@original";
            var content = JsonContent.Create(builder);
            await _http.PatchAsync(endpoint, content);
        }

        public async Task DeleteResponseAsync(string token)
        {
            var endpoint = $"/webhooks/{_client.CurrentApplication.Id}/{token}/messages/@original";
            await _http.DeleteAsync(endpoint);
        }
    }

    internal class InteractionCreatePayload
    {
        [JsonProperty("type")]
        public DiscordInteractionResponseType Type { get; set; }
        
        [JsonProperty("data")]
        public DiscordInteractionResponseBuilder? Data { get; set; }
    }
}