using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using UrfRidersBot.Library;

namespace UrfRidersBot.ConsoleUI.Modules
{
    public class PublicModule : BaseModule
    {
        public EmoteConfiguration Emotes { get; set; } = null!;
        
        [Command("ping")]
        public async Task Ping()
        {
            var embed = Embed.Basic(title: "Pong!");
            
            var stopwatch = Stopwatch.StartNew();
            var message = await ReplyAsync(embed: embed.Build());
            stopwatch.Stop();

            embed.WithFooter($"{stopwatch.ElapsedMilliseconds} ms");
            await message.ModifyAsync(x => x.Embed = embed.Build());
        }
        
        [Command("ask")]
        [Alias("yn", "question")]
        public async Task<RuntimeResult> Ask([Remainder]string question)
        {
            var sb = new StringBuilder();
            foreach (var mentionedRole in Context.Message.MentionedRoles)
            {
                sb.Append($"{mentionedRole.Mention}");
                question = question.Replace(mentionedRole.Mention, "");
            }

            question = question.Trim();

            var embed = Embed.Basic()
                .WithAuthor($"{Context.User.Username} has asked a question:", Context.User.GetAvatarUrl())
                .WithDescription(question);

            var message = await ReplyAsync(sb.ToString(), embed: embed.Build());
            _ = message.AddReactionAsync(Emotes.ThumbsUp);
            _ = message.AddReactionAsync(Emotes.ThumbsDown);

            // Just a test - do not use
            var yesList = new List<IUser>();
            var noList = new List<IUser>();
            Context.Client.ReactionAdded += async (msg, channel, reaction) =>
            {
                if (msg.Id != message.Id)
                    return;
                if (reaction.UserId == Context.Client.CurrentUser.Id)
                    return;
                if (reaction.Emote.Equals(Emotes.ThumbsUp))
                    yesList.Add(reaction.User.Value);
                else if (reaction.Emote.Equals(Emotes.ThumbsDown))
                    noList.Add(reaction.User.Value);
                
                var embed = Embed.Basic()
                    .WithAuthor($"{Context.User.Username} has asked a question:", Context.User.GetAvatarUrl())
                    .WithDescription(question);

                if (yesList.Count > 0)
                    embed.AddField(Emotes.ThumbsUp.ToString(), string.Join("\n", yesList.Select(x => x.Mention)), true);
                if (noList.Count > 0)
                    embed.AddField(Emotes.ThumbsDown.ToString(), string.Join("\n", noList.Select(x => x.Mention)), true);

                await message.ModifyAsync(x => x.Embed = embed.Build());
            };
            Context.Client.ReactionRemoved += async (msg, channel, reaction) =>
            {
                if (msg.Id != message.Id)
                    return;
                if (reaction.UserId == Context.Client.CurrentUser.Id)
                    return;
                if (reaction.Emote.Equals(Emotes.ThumbsUp))
                    yesList.Remove(reaction.User.Value);
                else if (reaction.Emote.Equals(Emotes.ThumbsDown))
                    noList.Remove(reaction.User.Value);

                var embed = Embed.Basic()
                    .WithAuthor($"{Context.User.Username} has asked a question:", Context.User.GetAvatarUrl())
                    .WithDescription(question);

                if (yesList.Count > 0)
                    embed.AddField(Emotes.ThumbsUp.ToString(), string.Join("\n", yesList.Select(x => x.Mention)), true);
                if (noList.Count > 0)
                    embed.AddField(Emotes.ThumbsDown.ToString(), string.Join("\n", noList.Select(x => x.Mention)), true);

                await message.ModifyAsync(x => x.Embed = embed.Build());
            };

            return CommandResult.FromEmpty(true);
        }

    }
}