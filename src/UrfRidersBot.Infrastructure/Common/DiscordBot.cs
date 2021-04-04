using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Hosting;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Commands.Services;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Infrastructure.Common
{
    internal class DiscordBot : IHostedService
    {
        private readonly DiscordClient _client;
        private readonly IBotInformationService _botInfo;
        private readonly IInteractionService _service;

        public DiscordBot(DiscordClient client, IBotInformationService botInfo, IInteractionService service)
        {
            _client = client;
            _botInfo = botInfo;
            _service = service;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client.InteractionCreated += OnInteractionCreated;
            
            _botInfo.SetStartTime(DateTimeOffset.Now);
            await _client.ConnectAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _client.InteractionCreated -= OnInteractionCreated;
            
            await _client.DisconnectAsync();
        }

        private async Task OnInteractionCreated(DiscordClient sender, InteractionCreateEventArgs e)
        {
            // Slash Commands Testing
            if (e.Interaction.Type == InteractionType.Ping)
                return;

            var context = new CommandContext(_client, e.Interaction, _service);
            switch (e.Interaction.Data.Name)
            {
                case "hello":
                    await HandleHelloCommandAsync(context);
                    break;
                case "whois":
                    await HandleWhoisCommandAsync(context);
                    break;
                default:
                    return;
            }
        }

        private async Task HandleHelloCommandAsync(CommandContext context)
        {
            var embed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = $"{context.User.Username} says hello! 👋",
                    IconUrl = context.User.AvatarUrl
                }
            };

            if (context.Member is not null && context.Member.Color.Value != default)
            {
                embed.WithColor(context.Member.Color);
            }

            await context.CreateResponseAsync(embed);
        }

        private async Task HandleWhoisCommandAsync(CommandContext context)
        {
            var userOption = context.Interaction.Data.Options.First(o => o.Name == "user");
            var user = context.Interaction.Data.Resolved.Members[(ulong)userOption.Value];

            var embed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = user.Username,
                    IconUrl = user.AvatarUrl
                },
                Color = user.Color.Value != 0
                    ? new Optional<DiscordColor>(user.Color)
                    : new Optional<DiscordColor>(),
                Description = "Must be someone nice."
            };

            await context.CreateEphemeralResponseAsync("Must be someone nice.");
        }
    }
}