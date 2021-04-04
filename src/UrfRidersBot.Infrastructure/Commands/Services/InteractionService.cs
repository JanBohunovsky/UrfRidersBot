using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Net;
using DSharpPlus.Net.Serialization;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Core.Commands.Built;
using UrfRidersBot.Core.Commands.Models;
using UrfRidersBot.Core.Commands.Services;
using UrfRidersBot.Infrastructure.Commands.Models;

namespace UrfRidersBot.Infrastructure.Commands.Services
{
    public class InteractionService : IInteractionService
    {
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
            await DoRequestAsync(endpoint, RestRequestMethod.POST, payload);
        }

        public async Task EditResponseAsync(string token, DiscordInteractionResponseBuilder builder)
        {
            var endpoint = $"webhooks/{_client.CurrentApplication.Id}/{token}/messages/@original";
            await DoRequestAsync(endpoint, RestRequestMethod.PATCH, builder);
        }

        public async Task DeleteResponseAsync(string token)
        {
            var endpoint = $"webhooks/{_client.CurrentApplication.Id}/{token}/messages/@original";
            await DoRequestAsync(endpoint, RestRequestMethod.DELETE);
        }

        public async Task RegisterCommandsAsync(IEnumerable<SlashCommand> commands, ulong? guildId = null)
        {
            var endpoint = guildId is null
                ? $"applications/{_client.CurrentApplication.Id}/commands"
                : $"applications/{_client.CurrentApplication.Id}/guilds/{guildId}/commands";

            var payload = CommandCreatePayload.FromCommands(commands.ToList());
            await DoRequestAsync(endpoint, RestRequestMethod.PUT, payload);
        }

        private async ValueTask<HttpResponseMessage> DoRequestAsync(string endpoint, RestRequestMethod method, object? payload = null)
        {
            var content = payload is not null
                ? new StringContent(DiscordJson.SerializeObject(payload), Encoding.UTF8, "application/json")
                : null;

            var requestTask = method switch
            {
                RestRequestMethod.GET    => _http.GetAsync(endpoint),
                RestRequestMethod.POST   => _http.PostAsync(endpoint, content!),
                RestRequestMethod.PUT    => _http.PutAsync(endpoint, content!),
                RestRequestMethod.PATCH  => _http.PatchAsync(endpoint, content!),
                RestRequestMethod.DELETE => _http.DeleteAsync(endpoint),
                _ => throw new InvalidOperationException("Invalid request method.")
            };

            var response = await requestTask;

            if (!response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
            
                _logger.LogError("Interaction http request failed ({StatusCode}: {StatusText}): {Response}",
                    (int)response.StatusCode, response.ReasonPhrase, responseString);
            }

            return response;
        }
        
        
    }
}