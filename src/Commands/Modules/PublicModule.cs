using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace UrfRidersBot
{
    [Description("Basic commands available to everyone.")]
    public class PublicModule : UrfRidersCommandModule
    {
        public EmoteConfiguration Emotes { get; set; } = null!;
        
        [Command("ping")]
        [Description("Get bot's WebSocket latency and response time.")]
        public async Task Ping(CommandContext ctx)
        {
            var embed = EmbedService
                .CreateBotInfo()
                .AddField("WebSocket", $"{ctx.Client.Ping} ms", true);

            var stopwatch = Stopwatch.StartNew();
            var message = await ctx.RespondAsync(embed.Build());
            stopwatch.Stop();

            embed.AddField("Response", $"{stopwatch.ElapsedMilliseconds} ms", true);
            await message.ModifyAsync(embed.Build());
        }
        
        [Command("ask")]
        [Aliases("yn", "question")]
        [Description("Ask a question! This will send a message with your question and with two reactions to respond 'yes' or 'no'.")]
        public async Task Ask(CommandContext ctx, [RemainingText][Description("Your question to other users.")] string question)
        {
            // TODO: Use interactive service here
            var sb = new StringBuilder();
            foreach (var mentionedRole in ctx.Message.MentionedRoles)
            {
                sb.Append($"{mentionedRole.Mention}");
                question = question.Replace(mentionedRole.Mention, "");
            }

            question = question.Trim();

            var embed = new DiscordEmbedBuilder
            {
                Color = UrfRidersColor.Cyan,
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = $"{ctx.User.Username} has asked a question:",
                    IconUrl = ctx.User.GetAvatarUrl(ImageFormat.Auto),
                },
                Description = question,
            };

            // Send message and add reactions
            var message = await ctx.RespondAsync(sb.ToString(), embed.Build());
            var emoteAgree = Emotes.Yes;
            var emoteDisagree = Emotes.No;
            _ = message.CreateReactionAsync(emoteAgree);
            _ = message.CreateReactionAsync(emoteDisagree);

            // Delete the original message to keep it clean.
            _ = ctx.Message.DeleteAsync();

            // Just a test - refactor later
            var yesList = new List<DiscordUser>();
            var noList = new List<DiscordUser>();
            ctx.Client.MessageReactionAdded += async (sender, e) =>
            {
                if (e.Message.Id != message.Id)
                    return;
                if (e.User.IsCurrent)
                    return;
                
                if (e.Emoji == emoteAgree)
                    yesList.Add(e.User);
                else if (e.Emoji == emoteDisagree)
                    noList.Add(e.User);
                
                var embed = new DiscordEmbedBuilder
                {
                    Color = UrfRidersColor.Cyan,
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = $"{ctx.User.Username} has asked a question:",
                        IconUrl = ctx.User.GetAvatarUrl(ImageFormat.Auto),
                    },
                    Description = question,
                };

                if (yesList.Count > 0)
                    embed.AddField(emoteAgree.ToString(), string.Join("\n", yesList.Select(u => u.Mention)), true);
                if (noList.Count > 0)
                    embed.AddField(emoteDisagree.ToString(), string.Join("\n", noList.Select(u => u.Mention)), true);

                await message.ModifyAsync(embed.Build());
            };

            ctx.Client.MessageReactionRemoved += async (sender, e) =>
            {
                if (e.Message.Id != message.Id)
                    return;
                if (e.User.IsCurrent)
                    return;
                
                if (e.Emoji == emoteAgree)
                    yesList.Remove(e.User);
                else if (e.Emoji == emoteDisagree)
                    noList.Remove(e.User);
                
                var embed = new DiscordEmbedBuilder
                {
                    Color = UrfRidersColor.Cyan,
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = $"{ctx.User.Username} has asked a question:",
                        IconUrl = ctx.User.GetAvatarUrl(ImageFormat.Auto),
                    },
                    Description = question,
                };

                if (yesList.Count > 0)
                    embed.AddField(emoteAgree.ToString(), string.Join("\n", yesList.Select(u => u.Mention)), true);
                if (noList.Count > 0)
                    embed.AddField(emoteDisagree.ToString(), string.Join("\n", noList.Select(u => u.Mention)), true);

                await message.ModifyAsync(embed.Build());
            };
        }

    }
}