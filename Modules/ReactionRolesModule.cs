using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using UrfRiders.Attributes.Preconditions;
using UrfRiders.Data;
using UrfRiders.Interactive;
using UrfRiders.Services;
using UrfRiders.Util;

namespace UrfRiders.Modules
{
    [Name("Reaction Roles")]
    [RequireContext(ContextType.Guild)]
    [RequireLevel(PermissionLevel.Admin)]
    [Group("reactionroles")]
    [Alias("rr")]
    public class ReactionRolesModule : BaseModule
    {
        public InteractiveService InteractiveService { get; set; }
        public ILogger<ReactionRolesModule> Logger { get; set; }

        private EmbedBuilder BaseEmbed => new EmbedBuilder().WithColor(Program.Color).WithTitle("Reaction Roles");

        private IMessageChannel Channel
        {
            get
            {
                var id = Settings.ReactionRolesChannel ?? Context.Channel.Id;
                if (Context.Client.GetChannel(id) is IMessageChannel channel)
                    return channel;
                Logger.LogInformation("Invalid channel, using current one.");
                return Context.Channel;
            }
            set => Settings.ReactionRolesChannel = value.Id;
        }

        [Command]
        [Priority(1)]
        public async Task ModuleHelp()
        {
            var typeBuilder = new StringBuilder();
            typeBuilder.AppendLine("`normal` - React to add role, remove reaction to remove role.");
            typeBuilder.AppendLine("`once  ` - React to add role.");
            typeBuilder.AppendLine("`remove` - React to remove role.");


            var embedBuilder = BaseEmbed
                .AddField("Set Channel", $"`{Settings.Prefix}rr channel <channel>`")
                .AddField("Set Message", $"`{Settings.Prefix}rr message <message id>`")
                .AddField("Add Role", $"`{Settings.Prefix}rr add <type> <emote> <role>`")
                .AddField("Remove Role", $"`{Settings.Prefix}rr remove <role>`")
                .AddField("Clear Roles", $"`{Settings.Prefix}rr clear`")
                .AddField("Available types", typeBuilder.ToString());

            if (Channel is ITextChannel channel)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Selected channel: {channel.Mention}");
                var message = await GetMessage(false);
                if (message != null)
                    sb.AppendLine($"Selected message: [{message.Id}]({message.GetJumpUrl()} \"{message.Author} in #{channel.Name}\")");
                embedBuilder.WithDescription(sb.ToString());
            }

            await ReplyAsync(embed: embedBuilder.Build());
        }

        [Command("channel")]
        [Priority(2)]
        public async Task SetChannel(ITextChannel channel)
        {
            Channel = channel;
            await ReplyAsync(embed: BaseEmbed.WithSuccess($"{channel.Mention} set as the active channel.").Build());
        }

        [Command("message")]
        [Priority(2)]
        public async Task SetMessage(ulong messageId)
        {
            var rawMessage = await Channel.GetMessageAsync(messageId);
            if (!(rawMessage is IUserMessage message))
            {
                await ReplyAsync(embed: BaseEmbed.WithError("Message not found in the active channel.").Build());
                return;
            }

            Settings.ReactionRolesMessage = message.Id;
            await ReplyAsync(embed: BaseEmbed.WithSuccess("Message set.").Build());
        }

        [Command("add")]
        [Priority(2)]
        public async Task AddReaction(ReactionRoleType type, string emoji, IRole role)
        {
            var message = await GetMessage();
            if (message == null)
                return;

            var emote = EmoteHelper.Parse(emoji);
            var handler = InteractiveService.GetHandler<RoleHandler>(message.Id) ?? new RoleHandler();

            try
            {
                await message.AddReactionAsync(emote);
            }
            catch (Exception)
            {
                await ReplyAsync(embed: BaseEmbed.WithError("Cannot use this emoji.").Build());
                return;
            }

            if (!handler.AddRole(emoji, role, type))
            {
                await ReplyAsync(embed: BaseEmbed.WithError("This role or emoji is already in use.").Build());
                return;
            }

            InteractiveService.SetHandler(message.Id, handler);
            await ReplyAsync(embed: BaseEmbed.WithSuccess($"{role.Mention} added with emoji {emote}").Build());
        }

        [Command("remove")]
        [Alias("delete")]
        [Priority(2)]
        public async Task RemoveReaction(IRole role)
        {
            var message = await GetMessage();
            if (message == null)
                return;

            var handler = InteractiveService.GetHandler<RoleHandler>(message.Id) ?? new RoleHandler();

            if (!handler.RemoveRole(role))
            {
                await ReplyAsync(embed: BaseEmbed.WithError("The message doesn't have this role.").Build());
            }

            if (handler.HasRoles)
                InteractiveService.SetHandler(message.Id, handler);
            else
                InteractiveService.RemoveHandler(message.Id);
            await ReplyAsync(embed: BaseEmbed.WithSuccess("The role has been removed.\nDo not forget to clean up the reactions.").Build());
        }

        [Command("clear")]
        [Alias("reset")]
        [Priority(2)]
        public async Task ClearReactions()
        {
            var message = await GetMessage();
            if (message == null)
                return;

            InteractiveService.RemoveHandler(message.Id);
            await ReplyAsync(embed: BaseEmbed.WithSuccess("The message is cleared.\nDo not forget to clean up the reactions.").Build());
        }

        private async Task<IUserMessage> GetMessage(bool notifyUser = true)
        {
            if (!Settings.ReactionRolesMessage.HasValue)
            {
                if (notifyUser)
                    await ReplyAsync(embed: BaseEmbed
                        .WithError("Please set a message before using this command.")
                        .AddField("Set Message", $"`{Settings.Prefix}rr message <message id>`")
                        .Build());
                return null;
            }

            var rawMessage = await Channel.GetMessageAsync(Settings.ReactionRolesMessage.Value);
            if (!(rawMessage is IUserMessage message))
            {
                if (notifyUser)
                    await ReplyAsync(embed: BaseEmbed.WithError("Message not found in the active channel.").Build());
                return null;
            }

            return message;
        }
    }
}