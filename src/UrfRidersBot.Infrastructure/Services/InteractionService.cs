using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Net.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UrfRidersBot.Core.Commands.Entities;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Infrastructure
{
    // Inspired by:
    // https://github.com/IDoEverything/DSharpPlusSlashCommands
    public class InteractionService : IInteractionService
    {
        private const string JsonMediaType = "application/json";
        
        private readonly DiscordClient _client;
        private readonly HttpClient _http;
        private readonly ILogger<InteractionService> _logger;

        public InteractionService(DiscordClient client, HttpClient http, ILogger<InteractionService> logger)
        {
            _client = client;
            _http = http;
            _logger = logger;
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

            var endpoint = $"interactions/{interactionId}/{token}/callback";
            var json = DiscordJson.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, JsonMediaType);
            var response = await _http.PostAsync(endpoint, content);
            
            if (!response.IsSuccessStatusCode)
            {
                await LogHttpErrorAsync(response, "create");
            }
        }

        public async Task EditResponseAsync(string token, DiscordInteractionResponseBuilder builder)
        {
            var endpoint = $"webhooks/{_client.CurrentApplication.Id}/{token}/messages/@original";
            var json = DiscordJson.SerializeObject(builder);
            var content = new StringContent(json, Encoding.UTF8, JsonMediaType);
            var response = await _http.PatchAsync(endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                await LogHttpErrorAsync(response, "edit");
            }
        }

        public async Task DeleteResponseAsync(string token)
        {
            var endpoint = $"webhooks/{_client.CurrentApplication.Id}/{token}/messages/@original";
            var response = await _http.DeleteAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                await LogHttpErrorAsync(response, "delete");
            }
        }

        private async Task LogHttpErrorAsync(HttpResponseMessage response, string action)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            
            _logger.LogError("Failed to {Action} interaction response ({StatusCode}: {StatusText}): {Response}",
                action, (int)response.StatusCode, response.ReasonPhrase, responseString);
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