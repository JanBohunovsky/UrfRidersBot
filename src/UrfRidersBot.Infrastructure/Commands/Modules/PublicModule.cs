using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Hosting;
using UrfRidersBot.Core;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Infrastructure.Commands.Modules
{
    [Description("Basic commands available to everyone.")]
    [ModuleLifespan(ModuleLifespan.Transient)]
    public class PublicModule : BaseCommandModule
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IColorRoleService _colorRoleService;
        private readonly IVersionService _versionService;

        public PublicModule(IHostEnvironment hostEnvironment,
            IColorRoleService colorRoleService,
            IVersionService versionService)
        {
            _hostEnvironment = hostEnvironment;
            _colorRoleService = colorRoleService;
            _versionService = versionService;
        }
        
        [Command("ping")]
        [Description("Get bot's WebSocket latency and response time.")]
        public async Task Ping(CommandContext ctx)
        {
            var embed = EmbedHelper
                .CreateBotInfo(ctx.Client)
                .AddField("WS Latency", $"{ctx.Client.Ping} ms", true);

            var stopwatch = Stopwatch.StartNew();
            var message = await ctx.RespondAsync(embed.Build());
            stopwatch.Stop();

            embed.AddField("Response Time", $"{stopwatch.ElapsedMilliseconds} ms", true);
            await message.ModifyAsync(embed.Build());
        }

        [RequireOwner]
        [Command("info")]
        public async Task Info(CommandContext ctx)
        {
            var embed = EmbedHelper.CreateBotInfo(ctx.Client);
            embed.AddField("Version", _versionService.BotVersion);
            embed.AddField("Environment", _hostEnvironment.EnvironmentName);
            embed.AddField("Host", Environment.MachineName);
            embed.AddField(".NET", Environment.Version.ToString(3));

            await ctx.RespondAsync(embed.Build());
        }

        [RequireGuildRank(GuildRank.Member)]
        [Command("color")]
        [Description("Give yourself a role with custom color.")]
        public async Task Color(CommandContext ctx, 
            [Description("Color you wish to use, for example: `#ff8000` or `255,128,0`.")]
            DiscordColor? color = null)
        {
            if (ctx.Member == null)
            {
                return;
            }

            if (color == null)
            {
                color = DiscordColor.None;
            }
            else if (color.Value.Value == DiscordColor.None.Value)
            {
                // If user entered pure black, set it to near black because pure black is treated as transparent.
                color = DiscordColor.Black;
            }

            await _colorRoleService.SetColorRoleAsync(ctx.Member, color.Value);
        }
        
        [RequireGuildRank(GuildRank.Member)]
        [Command("removeColor"), Aliases("clearColor")]
        [Description("Remove your custom color role.")]
        public async Task RemoveColor(CommandContext ctx)
        {
            if (ctx.Member == null)
            {
                return;
            }

            await _colorRoleService.RemoveColorRoleAsync(ctx.Member);
        }
        
        [Command("ask"), Aliases("yn", "question")]
        [Description("Ask a question!\nThis will send a message with your question and with two reactions to respond 'yes' or 'no'.")]
        public async Task Ask(CommandContext ctx, [RemainingText, Description("Your question to other users.")] string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                throw new InvalidOperationException("You need to enter a question.");
            
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
            var emoteAgree = DiscordEmoji.FromGuildEmote(ctx.Client, 791318099311329280); //Emotes.Yes;
            var emoteDisagree = DiscordEmoji.FromGuildEmote(ctx.Client, 791318099022315561); //Emotes.No;
            _ = message.CreateReactionAsync(emoteAgree);
            _ = message.CreateReactionAsync(emoteDisagree);

            // Delete the original message to keep it clean.
            _ = ctx.Message.DeleteAsync();

            // // Just a test - refactor later
            // var yesList = new List<DiscordUser>();
            // var noList = new List<DiscordUser>();
            // ctx.Client.MessageReactionAdded += async (sender, e) =>
            // {
            //     if (e.Message.Id != message.Id)
            //         return;
            //     if (e.User.IsCurrent)
            //         return;
            //     
            //     if (e.Emoji == emoteAgree)
            //         yesList.Add(e.User);
            //     else if (e.Emoji == emoteDisagree)
            //         noList.Add(e.User);
            //     
            //     var embed = new DiscordEmbedBuilder
            //     {
            //         Color = UrfRidersColor.Cyan,
            //         Author = new DiscordEmbedBuilder.EmbedAuthor
            //         {
            //             Name = $"{ctx.User.Username} has asked a question:",
            //             IconUrl = ctx.User.GetAvatarUrl(ImageFormat.Auto),
            //         },
            //         Description = question,
            //     };
            //
            //     if (yesList.Count > 0)
            //         embed.AddField(emoteAgree.ToString(), string.Join("\n", yesList.Select(u => u.Mention)), true);
            //     if (noList.Count > 0)
            //         embed.AddField(emoteDisagree.ToString(), string.Join("\n", noList.Select(u => u.Mention)), true);
            //
            //     await message.ModifyAsync(embed.Build());
            // };
            //
            // ctx.Client.MessageReactionRemoved += async (sender, e) =>
            // {
            //     if (e.Message.Id != message.Id)
            //         return;
            //     if (e.User.IsCurrent)
            //         return;
            //     
            //     if (e.Emoji == emoteAgree)
            //         yesList.Remove(e.User);
            //     else if (e.Emoji == emoteDisagree)
            //         noList.Remove(e.User);
            //     
            //     var embed = new DiscordEmbedBuilder
            //     {
            //         Color = UrfRidersColor.Cyan,
            //         Author = new DiscordEmbedBuilder.EmbedAuthor
            //         {
            //             Name = $"{ctx.User.Username} has asked a question:",
            //             IconUrl = ctx.User.GetAvatarUrl(ImageFormat.Auto),
            //         },
            //         Description = question,
            //     };
            //
            //     if (yesList.Count > 0)
            //         embed.AddField(emoteAgree.ToString(), string.Join("\n", yesList.Select(u => u.Mention)), true);
            //     if (noList.Count > 0)
            //         embed.AddField(emoteDisagree.ToString(), string.Join("\n", noList.Select(u => u.Mention)), true);
            //
            //     await message.ModifyAsync(embed.Build());
            // };
        }

    }
}