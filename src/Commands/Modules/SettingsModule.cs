﻿using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace UrfRidersBot
{
    [Group("settings")]
    [RequireGuildRank(GuildRank.Admin)]
    [Description("Read and modify a server specific variables.")]
    [ModuleLifespan(ModuleLifespan.Transient)]
    public class SettingsModule : UrfRidersCommandModule
    {
        public UrfRidersDbContext DbContext { get; set; } = null!;

        private const string NullText = "*null*";
        private const string CustomPrefixKey = "prefix";
        private const string MemberRoleKey = "member";
        private const string ModeratorRoleKey = "moderator";
        private const string AdminRoleKey = "admin";

        private GuildSettings _settings = null!;

        public override async Task BeforeExecutionAsync(CommandContext ctx)
        {
            _settings = await DbContext.GuildSettings.FindOrCreateAsync(ctx.Guild.Id, id => new GuildSettings(id));
        }

        [GroupCommand]
        [Description("A list of all available settings and their values.")]
        public async Task ShowAllSettings(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Color = UrfRidersColor.Cyan,
                Title = "Settings",
                Description = $"Current settings for {ctx.Guild.Name}.",
            };

            embed
                .AddField(
                    "Member role",
                    GetRoleMention(ctx, _settings.MemberRoleId) ?? NullText,
                    true)
                .AddField(
                    "Moderator role",
                    GetRoleMention(ctx, _settings.ModeratorRoleId) ?? NullText,
                    true)
                .AddField(
                    "Admin role",
                    GetRoleMention(ctx, _settings.AdminRoleId) ?? NullText,
                    true)
                .AddField(
                    "Custom prefix",
                    _settings.CustomPrefix.ToCode() ?? NullText,
                    true);

            await ctx.RespondAsync(embed);
        }

        [Command(CustomPrefixKey)]
        [Aliases("customPrefix")]
        [Description("Choose a custom prefix for this server.")]
        public async Task SetCustomPrefix(CommandContext ctx, [Description("Prefix to use on this server.")] string newPrefix)
        {
            _settings.CustomPrefix = newPrefix;
            await DbContext.SaveChangesAsync();

            var embed = EmbedService
                .CreateSuccess($"Prefix has been set to `{newPrefix}`.")
                .WithFooter("Tip: You can always invoke commands by mentioning me!")
                .Build();

            await ctx.RespondAsync(embed);
        }

        [Command(MemberRoleKey)]
        [Description("Choose a role which will represent all members on this server.")]
        public async Task SetMemberRole(CommandContext ctx, [Description("Role that describes a verified/trusted user on the server.")] DiscordRole role)
        {
            _settings.MemberRoleId = role.Id;
            await DbContext.SaveChangesAsync();

            var embed = EmbedService.CreateSuccess($"Member role has been set to {role.Mention}.").Build();
            await ctx.RespondAsync(embed);
        }

        [Command(ModeratorRoleKey)]
        [Description("Choose a role which will represent all moderators on this server.")]
        public async Task SetModeratorRole(CommandContext ctx, [Description("Role that describes a server moderator.")] DiscordRole role)
        {
            _settings.ModeratorRoleId = role.Id;
            await DbContext.SaveChangesAsync();

            var embed = EmbedService.CreateSuccess($"Moderator role has been set to {role.Mention}.").Build();
            await ctx.RespondAsync(embed);
        }

        [Command(AdminRoleKey)]
        [Description("Choose a role which will represent all admin on this server.")]
        public async Task SetAdminRole(CommandContext ctx, [Description("Role that describes a server admin.")] DiscordRole role)
        {
            _settings.AdminRoleId = role.Id;
            await DbContext.SaveChangesAsync();

            var embed = EmbedService.CreateSuccess($"Admin role has been set to {role.Mention}.").Build();
            await ctx.RespondAsync(embed);
        }

        [Command("reset")]
        [Description("Reset a specific setting")]
        public async Task ResetSetting(CommandContext ctx, [Description("Setting key, use `help settings` if you don't know.")] string key)
        {
            switch (key.ToLower())
            {
                case CustomPrefixKey:
                    _settings.CustomPrefix = null;
                    key = "Custom prefix";
                    break;
                case MemberRoleKey:
                    _settings.MemberRoleId = null;
                    key = "Member role";
                    break;
                case ModeratorRoleKey:
                    _settings.ModeratorRoleId = null;
                    key = "Moderator role";
                    break;
                case AdminRoleKey:
                    _settings.AdminRoleId = null;
                    key = "Admin role";
                    break;
                default:
                    await ctx.RespondAsync(EmbedService.CreateError("Setting not found.").Build());
                    return;
            }

            await DbContext.SaveChangesAsync();

            var embed = EmbedService.CreateSuccess($"{key} has been reset.").Build();
            await ctx.RespondAsync(embed);
        }

        [Command("resetAll")]
        [Description("Reset all settings for current server")]
        public async Task ResetAllSettings(CommandContext ctx)
        {
            DbContext.Remove(_settings);
            await DbContext.SaveChangesAsync();

            var embed = EmbedService.CreateSuccess($"All settings for {ctx.Guild.Name} has been reset.").Build();
            await ctx.RespondAsync(embed);
        }

        private string? GetRoleMention(CommandContext ctx, ulong? roleId)
        {
            return roleId == null ? null : ctx.Guild.GetRole(roleId.Value).Mention;
        }
    }
}