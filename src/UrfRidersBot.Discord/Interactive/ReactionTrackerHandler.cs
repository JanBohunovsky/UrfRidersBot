using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;

namespace UrfRidersBot.Discord.Interactive
{
    public class ReactionTrackerHandler : IReactionHandler
    {
        private readonly DiscordClient _client;
        private readonly IDbContextFactory<UrfRidersDbContext> _dbContextFactory;

        public ReactionTrackerHandler(
            DiscordClient client,
            IDbContextFactory<UrfRidersDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            _client = client;
        }

        public async Task ReactionAdded(DiscordMessage message, DiscordUser user, DiscordEmoji emote)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Reaction added",
                Description = $"{user.Mention} has added reaction '{emote}' to a [message]({message.JumpLink})",
            };
            
            var trackingUser = await GetUser(message);
            await trackingUser.SendMessageAsync(embed.Build());
        }

        public async Task ReactionRemoved(DiscordMessage message, DiscordUser user, DiscordEmoji emote)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Reaction added",
                Description = $"{user.Mention} has removed reaction '{emote}' to a [message]({message.JumpLink})",
            };

            var trackingUser = await GetUser(message);
            await trackingUser.SendMessageAsync(embed.Build());
        }

        private async ValueTask<DiscordMember> GetUser(DiscordMessage message)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();
            var data = await dbContext.ReactionTrackerData.FindAsync(message.Id);
            if (data == null)
                throw new Exception($"No data found for {nameof(ReactionTrackerHandler)}");

            return await message.Channel.Guild.GetMemberAsync(data.UserId);
        }
    }
}