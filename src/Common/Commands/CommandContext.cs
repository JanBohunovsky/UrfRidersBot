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

        public DiscordGuild Guild => Interaction.Guild;
        public DiscordChannel Channel => Interaction.Channel;
        public DiscordUser User => Interaction.User;
        public DiscordMember Member => (DiscordMember)User;
        
        public CommandContext(DiscordClient client, DiscordInteraction interaction)
        {
            if (interaction.Guild is null)
            {
                throw new ArgumentException("DMs are not supported.", nameof(interaction));
            }

            Client = client;
            Interaction = interaction;
        }

        public Task RespondAsync(string content)
        {
            throw new NotImplementedException();
        }
    }
}