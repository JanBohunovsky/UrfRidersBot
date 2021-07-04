using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace UrfRidersBot.Common.Commands
{
    public class CommandContext
    {
        public DiscordClient Client { get; }
        public DiscordInteraction Interaction { get; }
        public bool IsEphemeral { get; }

        public DiscordGuild Guild => Interaction.Guild;
        public DiscordChannel Channel => Interaction.Channel;
        public DiscordUser User => Interaction.User;
        public DiscordMember Member => (DiscordMember)User;
        
        public CommandContext(DiscordClient client, DiscordInteraction interaction, bool isEphemeral)
        {
            if (interaction.Guild is null)
            {
                throw new ArgumentException("Direct messages are not supported.", nameof(interaction));
            }

            Client = client;
            Interaction = interaction;
            IsEphemeral = isEphemeral;
        }

        public async ValueTask<DiscordMessage> RespondAsync(DiscordFollowupMessageBuilder builder)
        {
            if (IsEphemeral && builder.Embeds.Count > 0)
            {
                throw new InvalidOperationException("Embeds are not supported in ephemeral messages.");
            }

            return await Interaction.CreateFollowupMessageAsync(builder);
        }

        public async ValueTask<DiscordMessage> RespondAsync(string content)
        {
            var builder = new DiscordFollowupMessageBuilder()
                .WithContent(content)
                .AddMentions(Mentions.All);

            return await RespondAsync(builder);
        }

        public async ValueTask<DiscordMessage> RespondAsync(DiscordEmbed embed)
        {
            var builder = new DiscordFollowupMessageBuilder()
                .AddEmbed(embed);

            return await RespondAsync(builder);
        }

        public async ValueTask<DiscordMessage> RespondAsync(string content, DiscordEmbed embed)
        {
            var builder = new DiscordFollowupMessageBuilder()
                .WithContent(content)
                .AddEmbed(embed)
                .AddMentions(Mentions.All);

            return await RespondAsync(builder);
        }
    }
}