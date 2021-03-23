using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Net.Serialization;
using Microsoft.Extensions.Hosting;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Infrastructure.HostedServices
{
    internal class DiscordBot : IHostedService
    {
        private readonly DiscordClient _client;
        private readonly IBotInformationService _botInfo;
        private readonly HttpClient _http;

        public DiscordBot(DiscordClient client, IBotInformationService botInfo, HttpClient http)
        {
            _client = client;
            _botInfo = botInfo;
            _http = http;
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

            switch (e.Interaction.Data.Name)
            {
                case "hello":
                    await HandleHelloCommandAsync(e.Interaction, e.Interaction.Data);
                    break;
                case "whois":
                    await HandleWhoisCommandAsync(e.Interaction, e.Interaction.Data);
                    break;
                default:
                    return;
            }
        }

        private async Task RespondAsync(DiscordInteraction interaction, string? content = null, DiscordEmbed? embed = null, bool ephemeral = false)
        {
            var response = new
            {
                type = 4,
                data = new
                {
                    content = content,
                    embeds = new[] { embed },
                    flags = ephemeral ? 64 : 0
                }
            };

            var uri = $"https://discord.com/api/v8/interactions/{interaction.Id}/{interaction.Token}/callback";
            var responseContent = new StringContent(DiscordJson.SerializeObject(response), Encoding.UTF8, "application/json");
            await _http.PostAsync(uri, responseContent);
        }

        private async Task HandleHelloCommandAsync(DiscordInteraction interaction, DiscordInteractionData data)
        {
            var member = (DiscordMember)interaction.User;
            var embed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = $"{interaction.User.Username} says hello! 👋",
                    IconUrl = interaction.User.AvatarUrl
                },
                Color = member.Color.Value != 0
                    ? new Optional<DiscordColor>(member.Color)
                    : new Optional<DiscordColor>(),
            };

            // var message = await interaction.Channel.SendMessageAsync(embed);
            // await message.CreateReactionAsync(DiscordEmoji.FromUnicode("👋"));
            await RespondAsync(interaction, embed: embed);
        }

        private async Task HandleWhoisCommandAsync(DiscordInteraction interaction, DiscordInteractionData data)
        {
            var userOption = data.Options.First(o => o.Name == "user");
            var user = data.Resolved.Members[(ulong)userOption.Value];

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
                Description = "Must be someone nice.",
                // Footer = new DiscordEmbedBuilder.EmbedFooter
                // {
                //     Text = $"This lookup was done by {interaction.User.Username}",
                //     IconUrl = interaction.User.AvatarUrl
                // }
            };

            //await interaction.Channel.SendMessageAsync(embed);
            await RespondAsync(interaction, "Must be someone nice", ephemeral: true);
        }
    }
}