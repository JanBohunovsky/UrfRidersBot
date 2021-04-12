using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Commands.Attributes;
using UrfRidersBot.Core.Common;
using UrfRidersBot.Core.ReactionRoles;
using UrfRidersBot.Infrastructure.Common;

namespace UrfRidersBot.Commands.ReactionRoles
{
    public class Remove : ICommand<ReactionRolesGroup>
    {
        private readonly IUnitOfWork _unitOfWork;

        public bool Ephemeral => true;
        public string Name => "remove";
        public string Description => "Remove a self-assignable role from message.";
        
        [Parameter("message-link", "Link to the message.")]
        public string MessageLink { get; set; } = "";

        [Parameter("role", "The role you wish to remove from being self-assignable in a message.")]
        public DiscordRole Role { get; set; } = null!;

        public Remove(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async ValueTask<CommandResult> HandleAsync(ICommandContext context)
        {
            var message = await DiscordMessageHelper.FromLinkAsync(context.Guild, MessageLink);
            if (message is null)
            {
                return CommandResult.InvalidParameter("Invalid message link.");
            }
            
            var emoji = await _unitOfWork.ReactionRoles.GetEmojiAsync(context.Client, message, Role);
            var messageLink = Markdown.Link("message", message.JumpLink.ToString());
            
            if (emoji == null)
            {
                return CommandResult.InvalidOperation($"This {messageLink} does not have self-assignable role {Role.Mention}.");
            }
            
            await message.DeleteReactionsEmojiAsync(emoji);

            var reactionRole = new ReactionRole(message, emoji, Role);
            _unitOfWork.ReactionRoles.Remove(reactionRole);
            await _unitOfWork.CompleteAsync();

            return CommandResult.Success($"Removed self-assignable role {Role.Mention} from {messageLink}.");
        }
    }
}