using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Commands.Entities;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Core.Commands
{
    public class InteractionContext
    {
        private readonly IInteractionService _service;

        public DiscordClient Client { get; }
        public DiscordInteraction Interaction { get; }

        public DiscordChannel Channel => Interaction.Channel;
        public DiscordUser User => Interaction.User;
        public DiscordGuild? Guild => Interaction.Guild;
        public DiscordMember? Member => User as DiscordMember;

        public InteractionContext(DiscordClient client, DiscordInteraction interaction, IInteractionService service)
        {
            _service = service;
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

        public async Task CreateResponseAsync(DiscordInteractionResponseBuilder? builder = null,
            DiscordInteractionResponseType type = DiscordInteractionResponseType.ChannelMessageWithSource)
        {
            await _service.CreateResponseAsync(Interaction.Id, Interaction.Token, type, builder);
        }

        public async Task CreateResponseAsync(string content,
            DiscordInteractionResponseType type = DiscordInteractionResponseType.ChannelMessageWithSource)
        {
            var builder = new DiscordInteractionResponseBuilder()
                .WithContent(content)
                .WithMentions(Mentions.All);
            
            await CreateResponseAsync(builder, type);
        }

        public async Task CreateResponseAsync(DiscordEmbed embed,
            DiscordInteractionResponseType type = DiscordInteractionResponseType.ChannelMessageWithSource)
        {
            var builder = new DiscordInteractionResponseBuilder()
                .WithEmbeds(embed);

            await CreateResponseAsync(builder, type);
        }

        public async Task CreateResponseAsync(string content, DiscordEmbed embed,
            DiscordInteractionResponseType type = DiscordInteractionResponseType.ChannelMessageWithSource)
        {
            var builder = new DiscordInteractionResponseBuilder()
                .WithContent(content)
                .WithEmbeds(embed)
                .WithMentions(Mentions.All);

            await CreateResponseAsync(builder, type);
        }

        public async Task EditResponseAsync(DiscordInteractionResponseBuilder builder)
        {
            await _service.EditResponseAsync(Interaction.Token, builder);
        }

        public async Task EditResponseAsync(string content)
        {
            var builder = new DiscordInteractionResponseBuilder()
                .WithContent(content)
                .WithMentions(Mentions.All);

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
                .WithEmbeds(embed)
                .WithMentions(Mentions.All);

            await EditResponseAsync(builder);
        }

        public async Task DeleteResponseAsync()
        {
            await _service.DeleteResponseAsync(Interaction.Token);
        }
    }
}