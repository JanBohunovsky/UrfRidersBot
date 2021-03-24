using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace UrfRidersBot.Infrastructure.HostedServices
{
    public class SlashCommandHostedService : IHostedService
    {
        private readonly DiscordClient _client;
        private readonly ILogger<SlashCommandHostedService> _logger;

        public SlashCommandHostedService(DiscordClient client, ILogger<SlashCommandHostedService> logger)
        {
            _client = client;
            _logger = logger;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _client.Ready += OnReady;
            _client.InteractionCreated += OnInteractionCreated;
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _client.Ready -= OnReady;
            _client.InteractionCreated -= OnInteractionCreated;
            
            return Task.CompletedTask;
        }

        private Task OnReady(DiscordClient sender, ReadyEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await RegisterCommandsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An exception has occured while registering slash commands");
                }
            });
            
            return Task.CompletedTask;
        }

        private Task OnInteractionCreated(DiscordClient sender, InteractionCreateEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await HandleInteractionAsync(e);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An exception has occured while handling the interaction");
                }
            });

            return Task.CompletedTask;
        }

        private async Task RegisterCommandsAsync()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly is null)
            {
                throw new InvalidOperationException("What?");
            }
            
            var commandPayloads = new List<CreateCommandPayload>();
        }

        private async Task HandleInteractionAsync(InteractionCreateEventArgs e)
        {
            
        }
    }

    internal class CreateCommandPayload
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("options")]
        public List<DiscordApplicationCommandOption> Options { get; set; } = new();
    }
}