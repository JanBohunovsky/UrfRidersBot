using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace UrfRidersBot.Core.Commands
{
    public class CommandContext
    {
        public DiscordClient Client { get; }
        public DiscordInteraction Interaction { get; }

        public DiscordChannel Channel => Interaction.Channel;
        public DiscordUser User => Interaction.User;
        public DiscordGuild Guild => Interaction.Guild;
        public DiscordMember Member => (DiscordMember)User;

        public CommandContext(DiscordClient client, DiscordInteraction interaction)
        {
            if (interaction.Guild is null)
            {
                throw new ArgumentException("Only guild interactions are supported.", nameof(interaction));
            }
            
            Client = client;
            Interaction = interaction;
        }

        public async Task CreateEphemeralResponseAsync(string content)
        {
            var builder = new DiscordInteractionResponseBuilder()
                .WithContent(content)
                .AsEphemeral(true);

            await CreateResponseAsync(builder);
        }

        public async Task CreateResponseAsync(DiscordInteractionResponseBuilder? builder = null)
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
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
                .AddEmbed(embed);

            await CreateResponseAsync(builder);
        }

        public async Task CreateResponseAsync(string content, DiscordEmbed embed)
        {
            var builder = new DiscordInteractionResponseBuilder()
                .WithContent(content)
                .AddEmbed(embed);

            await CreateResponseAsync(builder);
        }

        public async Task EditResponseAsync(DiscordWebhookBuilder builder)
        {
            await Interaction.EditOriginalResponseAsync(builder);
        }

        public async Task EditResponseAsync(string content)
        {
            var builder = new DiscordWebhookBuilder()
                .WithContent(content);

            await EditResponseAsync(builder);
        }

        public async Task EditResponseAsync(DiscordEmbed embed)
        {
            var builder = new DiscordWebhookBuilder()
                .AddEmbed(embed);

            await EditResponseAsync(builder);
        }

        public async Task EditResponseAsync(string content, DiscordEmbed embed)
        {
            var builder = new DiscordWebhookBuilder()
                .WithContent(content)
                .AddEmbed(embed);

            await EditResponseAsync(builder);
        }

        public async Task DeleteResponseAsync()
        {
            await Interaction.DeleteOriginalResponseAsync();
        }
    }
}