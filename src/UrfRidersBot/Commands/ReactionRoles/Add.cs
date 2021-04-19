using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Commands.Attributes;
using UrfRidersBot.Core.Common;
using UrfRidersBot.Core.ReactionRoles;

namespace UrfRidersBot.Commands.ReactionRoles
{
    public class Add : ICommand<ReactionRolesGroup>
    {
        private readonly IReactionRoleRepository _repository;

        public bool Ephemeral => true;
        public string Name => "add";
        public string Description => "Add a self-assignable role to a message.";

        [Parameter("message-link", "Link to the message.")]
        public string MessageLink { get; set; } = "";

        [Parameter("emoji", "An emoji to represent the role (this will be added as a reaction).")]
        public string EmojiText { get; set; } = "";

        [Parameter("role", "The role you wish to be self-assignable.")]
        public DiscordRole Role { get; set; } = null!;

        public Add(IReactionRoleRepository repository)
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

            var emoji = DiscordEmojiHelper.Parse(context.Client, EmojiText);
            if (emoji is null)
            {
                return CommandResult.InvalidParameter("Invalid emoji.");
            }
            
            var reactionRole = new ReactionRole(message, Role, emoji);
            var result = await _repository.AddAsync(reactionRole);

            if (!result)
            {
                return CommandResult.InvalidOperation("This message already has this role or emoji.");
            }
            
            await message.CreateReactionAsync(emoji);
            
            var messageLink = Markdown.Link("message", message.JumpLink.ToString());
            return CommandResult.Success($"Added self-assignable role {Role.Mention} with emoji {emoji} to {messageLink}.");
        }
    }
}