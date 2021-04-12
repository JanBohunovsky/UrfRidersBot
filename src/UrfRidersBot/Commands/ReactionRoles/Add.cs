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
    public class Add : ICommand<ReactionRolesGroup>
    {
        private readonly IUnitOfWork _unitOfWork;

        public bool Ephemeral => true;
        public string Name => "add";
        public string Description => "Add a self-assignable role to a message.";

        [Parameter("message-link", "Link to the message.")]
        public string MessageLink { get; set; } = "";

        [Parameter("emoji", "An emoji to represent the role (this will be added as a reaction).")]
        public string EmojiText { get; set; } = "";

        [Parameter("role", "The role you wish to be self-assignable.")]
        public DiscordRole Role { get; set; } = null!;

        public Add(IUnitOfWork unitOfWork)
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

            var emoji = DiscordEmojiHelper.Parse(context.Client, EmojiText);
            if (emoji is null)
            {
                return CommandResult.InvalidParameter("Invalid emoji.");
            }
            
            // TODO: Check if there will be collision in db and show the user nice error message.
            var reactionRole = new ReactionRole(message, emoji, Role);
            await _unitOfWork.ReactionRoles.AddAsync(reactionRole);
            
            await message.CreateReactionAsync(emoji);
            await _unitOfWork.CompleteAsync();
            
            var messageLink = Markdown.Link("message", message.JumpLink.ToString());
            return CommandResult.Success($"Added self-assignable role {Role.Mention} with emoji {emoji} to {messageLink}.");
        }
    }
}