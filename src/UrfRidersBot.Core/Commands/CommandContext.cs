using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Commands.Models;
using UrfRidersBot.Core.Commands.Services;

namespace UrfRidersBot.Core.Commands
{
    public class CommandContext
    {
        private readonly IInteractionService _interactionService;

        public DiscordClient Client { get; }
        public DiscordInteraction Interaction { get; }

        public DiscordChannel Channel => Interaction.Channel;
        public DiscordUser User => Interaction.User;
        public DiscordGuild Guild => Interaction.Guild;
        public DiscordMember Member => (DiscordMember)User;

        public CommandContext(DiscordClient client, DiscordInteraction interaction, IInteractionService interactionService)
        {
            _interactionService = interactionService;
            Client = client;
            Interaction = interaction;
        }

        public async Task CreateEphemeralResponseAsync(string content)
        {
            var builder = new DiscordInteractionResponseBuilder()
                .WithContent(content)
                .WithFlags(InteractionFlags.Ephemeral);

            await CreateResponseAsync(builder);
        }

        public async Task CreateResponseAsync(DiscordInteractionResponseBuilder? builder = null)
        {
            await _interactionService.CreateResponseAsync(
                Interaction.Id,
                Interaction.Token,
                DiscordInteractionResponseType.ChannelMessageWithSource,
                builder);
        }

        public async Task CreateResponseAsync(string content)
        {
            var builder = new DiscordInteractionResponseBuilder()
                .WithContent(content);
            
            await CreateResponseAsync(builder);
        }

        public async Task CreateResponseAsync(DiscordEmbed embed)
        {
            var builder = new DiscordInteractionResponseBuilder()
                .WithEmbeds(embed);

            await CreateResponseAsync(builder);
        }

        public async Task CreateResponseAsync(string content, DiscordEmbed embed)
        {
            var builder = new DiscordInteractionResponseBuilder()
                .WithContent(content)
                .WithEmbeds(embed);

            await CreateResponseAsync(builder);
        }

        public async Task EditResponseAsync(DiscordInteractionResponseBuilder builder)
        {
            await _interactionService.EditResponseAsync(Interaction.Token, builder);
        }

        public async Task EditResponseAsync(string content)
        {
            var builder = new DiscordInteractionResponseBuilder()
                .WithContent(content);

            await EditResponseAsync(builder);
        }

        public async Task EditResponseAsync(DiscordEmbed embed)
        {
            var builder = new DiscordInteractionResponseBuilder()
                .WithEmbeds(embed);

            await EditResponseAsync(builder);
        }

        public async Task EditResponseAsync(string content, DiscordEmbed embed)
        {
            var builder = new DiscordInteractionResponseBuilder()
                .WithContent(content)
                .WithEmbeds(embed);

            await EditResponseAsync(builder);
        }

        public async Task DeleteResponseAsync()
        {
            await _interactionService.DeleteResponseAsync(Interaction.Token);
        }
    }
}