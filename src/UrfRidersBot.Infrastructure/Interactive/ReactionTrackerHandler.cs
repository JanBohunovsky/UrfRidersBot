using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Infrastructure.Interactive
{
    public class ReactionTrackerHandler : IReactionHandler
    {
        private readonly DiscordClient _client;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public ReactionTrackerHandler(DiscordClient client, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _client = client;
            _unitOfWorkFactory = unitOfWorkFactory;
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
            await using var unitOfWork = _unitOfWorkFactory.Create();
            var data = await unitOfWork.ReactionTrackerData.GetByMessageAsync(message);
            if (data == null)
                throw new Exception($"No data found for {nameof(ReactionTrackerHandler)}");

            return await message.Channel.Guild.GetMemberAsync(data.UserId);
        }
    }
}