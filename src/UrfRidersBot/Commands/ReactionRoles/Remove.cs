using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Common;
using UrfRidersBot.Core.ReactionRoles;

namespace UrfRidersBot.Commands.ReactionRoles
{
    public class Remove : ICommand<ReactionRolesGroup>
    {
        private readonly IReactionRoleRepository _repository;

        public bool Ephemeral => true;
        public string Name => "remove";
        public string Description => "Remove a self-assignable role from message.";
        
        [Parameter("message-link", "Link to the message.")]
        public string MessageLink { get; set; } = "";

        [Parameter("role", "The role you wish to remove from being self-assignable in a message.")]
        public DiscordRole Role { get; set; } = null!;

        public Remove(IReactionRoleRepository repository)
        {
            _repository = repository;
        }
        
        public async ValueTask<CommandResult> HandleAsync(ICommandContext context)
        {
            var message = await DiscordMessageHelper.FromLinkAsync(context.Guild, MessageLink);
            if (message is null)
            {
                return CommandResult.InvalidParameter("Invalid message link.");
            }
            
            var emoji = await _repository.GetEmojiAsync(message, Role);
            var messageLink = Markdown.Link("message", message.JumpLink.ToString());
            
            if (emoji == null)
            {
                return CommandResult.InvalidOperation($"This {messageLink} does not have self-assignable role {Role.Mention}.");
            }
            
            await message.DeleteReactionsEmojiAsync(emoji);
            await _repository.RemoveAsync(message, Role);

            return CommandResult.Success($"Removed self-assignable role {Role.Mention} from {messageLink}.");
        }
    }
}