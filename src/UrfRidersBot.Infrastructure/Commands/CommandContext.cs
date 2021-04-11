using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Commands;

namespace UrfRidersBot.Infrastructure.Commands
{
    public class CommandContext : ICommandContext
    {
        public DiscordClient Client { get; }
        public DiscordInteraction Interaction { get; }
        public bool IsEphemeral { get; }

        public DiscordChannel Channel => Interaction.Channel;
        public DiscordUser User => Interaction.User;
        public DiscordGuild Guild => Interaction.Guild;
        public DiscordMember Member => (DiscordMember)User;

        public CommandContext(DiscordClient client, DiscordInteraction interaction, bool isEphemeral)
        {
            if (interaction.Guild is null)
            {
                throw new ArgumentException("Only guild interactions are supported.", nameof(interaction));
            }
            
            Client = client;
            Interaction = interaction;
            IsEphemeral = isEphemeral;
        }

        public async ValueTask<DiscordMessage> RespondAsync(DiscordFollowupMessageBuilder? builder = null)
        {
            if (builder is not null && builder.Embeds.Count > 0 && IsEphemeral)
            {
                throw new InvalidOperationException("Embeds are not supported in ephemeral messages.");
            }
            
            return await Interaction.CreateFollowupMessageAsync(builder);
        }

        public async ValueTask<DiscordMessage> RespondAsync(string content)
        {
            var builder = new DiscordFollowupMessageBuilder()
                .WithContent(content);
            
            return await RespondAsync(builder);
        }

        public async ValueTask<DiscordMessage> RespondAsync(DiscordEmbed embed)
        {
            if (IsEphemeral)
            {
                throw new InvalidOperationException("Embeds are not supported in ephemeral messages.");
            }
            
            var builder = new DiscordFollowupMessageBuilder()
                .AddEmbed(embed);

            return await RespondAsync(builder);
        }

        public async ValueTask<DiscordMessage> RespondAsync(string content, DiscordEmbed embed)
        {
            if (IsEphemeral)
            {
                throw new InvalidOperationException("Embeds are not supported in ephemeral messages.");
            }
            
            var builder = new DiscordFollowupMessageBuilder()
                .WithContent(content)
                .AddEmbed(embed);

            return await RespondAsync(builder);
        }
    }
}