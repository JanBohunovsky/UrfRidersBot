using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using UrfRidersBot.Core;
using UrfRidersBot.Core.Entities;
using UrfRidersBot.Core.Interfaces;
using UrfRidersBot.Infrastructure.Commands.Attributes;

namespace UrfRidersBot.Infrastructure.Commands.Modules
{
    [Group("reactionRoles"), Aliases("rr")]
    [Description("Set up self-assignable roles.")]
    [RequireGuildRank(GuildRank.Admin)]
    [ModuleLifespan(ModuleLifespan.Transient)]
    public class ReactionRolesModule : BaseCommandModule
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReactionRolesModule(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Command("add")]
        [Description("Add a self-assignable role to target message.")]
        public async Task AddRole(CommandContext ctx, DiscordMessage message, DiscordEmoji emoji, DiscordRole role)
        {
            // TODO: Check if there will be collision in db and show the user nice error message.
            var reactionRole = new ReactionRole(message, emoji, role);
            await _unitOfWork.ReactionRoles.AddAsync(reactionRole);
            await _unitOfWork.CompleteAsync();

            await message.CreateReactionAsync(emoji);

            var messageLink = Markdown.Link("message", message.JumpLink.ToString());
            var embed = EmbedHelper.CreateSuccess(
                $"Added self-assignable role {role.Mention} with emoji {emoji} to {messageLink}.");
            await ctx.RespondAsync(embed);
        }

        [Command("remove")]
        [Description("Remove a self-assignable role from message.")]
        public async Task RemoveRole(CommandContext ctx, DiscordMessage message, DiscordRole role)
        {
            var emoji = await _unitOfWork.ReactionRoles.GetEmojiAsync(ctx.Client, message, role);
            var messageLink = Markdown.Link("message", message.JumpLink.ToString());
            
            if (emoji == null)
            {
                await ctx.RespondAsync(
                    EmbedHelper.CreateError($"This {messageLink} does not have self-assignable role {role.Mention}."));
                return;
            }
            
            await message.DeleteReactionsEmojiAsync(emoji);

            var reactionRole = new ReactionRole(message, emoji, role);
            _unitOfWork.ReactionRoles.Remove(reactionRole);
            await _unitOfWork.CompleteAsync();

            var embed = EmbedHelper.CreateSuccess(
                $"Removed self-assignable role {role.Mention} from {messageLink}.");
            await ctx.RespondAsync(embed);
        }
    }
}