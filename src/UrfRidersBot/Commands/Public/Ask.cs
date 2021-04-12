using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Commands.Attributes;
using UrfRidersBot.Infrastructure.Common;

namespace UrfRidersBot.Commands.Public
{
    public class Ask : ICommand
    {
        public bool Ephemeral => false;
        public string Name => "ask";
        public string Description => "This will send your question with 'yes' and 'no' reactions.";

        [Parameter("question", "Your question.")]
        public string Question { get; set; } = "";

        [Parameter("target-role", "If you want, you can send the question to a specific role.", true)]
        public DiscordRole? TargetRole { get; set; } = null;
        
        public async ValueTask<CommandResult> HandleAsync(ICommandContext context)
        {
            var embed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = context.User.Username,
                    IconUrl = context.User.AvatarUrl
                },
                Description = Question,
                Color = context.Member.Color
            };

            var builder = new DiscordFollowupMessageBuilder()
                .AddEmbed(embed);

            if (TargetRole is not null)
            {
                builder
                    .WithContent(TargetRole.Mention)
                    .AddMention(new RoleMention(TargetRole));
            }

            var message = await context.RespondAsync(builder);

            var emoteYes = DiscordEmojiHelper.Parse(context.Client, UrfRidersEmotes.Checkmark);
            var emoteNo = DiscordEmojiHelper.Parse(context.Client, UrfRidersEmotes.Crossmark);

            _ = message.CreateReactionAsync(emoteYes);
            _ = message.CreateReactionAsync(emoteNo);
            
            // TODO: Show list of users that reacted in the message.
            
            return CommandResult.NoAction;
        }
    }
}