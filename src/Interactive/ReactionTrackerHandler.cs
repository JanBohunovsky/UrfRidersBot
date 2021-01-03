using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace UrfRidersBot.Interactive
{
    public class ReactionTrackerHandler : IReactionHandler
    {
        private readonly EmbedService _embedService;
        private readonly DiscordSocketClient _client;
        private readonly IDbContextFactory<UrfRidersDbContext> _dbContextFactory;

        public ReactionTrackerHandler(
            EmbedService embedService,
            DiscordSocketClient client,
            IDbContextFactory<UrfRidersDbContext> dbContextFactory)
        {
            _embedService = embedService;
            _client = client;
            _dbContextFactory = dbContextFactory;
        }

        public async Task ReactionAdded(IUserMessage message, IUser user, IEmote emote)
        {
            var embed = _embedService
                .CreateBasic(
                    $"{user.Mention} has added reaction '{emote}' to the [message]({message.GetJumpUrl()}).",
                    "Reaction added")
                .Build();
            
            var trackingUser = await GetUser(message.Id);
            await trackingUser.SendMessageAsync(embed: embed);
        }

        public async Task ReactionRemoved(IUserMessage message, IUser user, IEmote emote)
        {
            var embed = _embedService
                .CreateBasic(
                    $"{user.Mention} has removed reaction '{emote}' to the [message]({message.GetJumpUrl()}).",
                    "Reaction removed")
                .Build();

            var trackingUser = await GetUser(message.Id);
            await trackingUser.SendMessageAsync(embed: embed);
        }

        private async ValueTask<IUser> GetUser(ulong messageId)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();
            var data = await dbContext.ReactionTrackerData.FindAsync(messageId);
            if (data == null)
                throw new Exception($"No data found for {nameof(ReactionTrackerHandler)}");

            return _client.GetUser(data.UserId);
        }
    }
}