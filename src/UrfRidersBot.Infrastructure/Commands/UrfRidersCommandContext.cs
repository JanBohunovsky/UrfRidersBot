using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Qmmands;

namespace UrfRidersBot.Infrastructure.Commands
{
    public class UrfRidersCommandContext : CommandContext
    {
        public string Prefix { get; }
        public DiscordClient Client { get; }
        public DiscordMessage Message { get; }
        public DiscordUser User => Message.Author;
        public DiscordChannel Channel => Message.Channel;
        public DiscordGuild Guild => Channel.Guild;
        public DiscordMember Member => (DiscordMember)User;
        
        public UrfRidersCommandContext(DiscordMessage message, DiscordClient client, string prefix, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Prefix = prefix;
            Client = client;
            Message = message;
        }

        public async ValueTask<DiscordMessage> RespondAsync(string content)
            => await Message.RespondAsync(content);
        
        public async ValueTask<DiscordMessage> RespondAsync(DiscordEmbed embed)
            => await Message.RespondAsync(embed);

        public async ValueTask<DiscordMessage> RespondAsync(string content, DiscordEmbed embed)
            => await Message.RespondAsync(content, embed);

        public async ValueTask<DiscordMessage> RespondAsync(DiscordMessageBuilder builder)
            => await Message.RespondAsync(builder);

        public async Task TriggerTypingAsync()
            => await Channel.TriggerTypingAsync();
    }
}