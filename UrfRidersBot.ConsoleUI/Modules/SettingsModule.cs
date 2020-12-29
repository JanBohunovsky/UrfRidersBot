using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using UrfRidersBot.Library;

namespace UrfRidersBot.ConsoleUI.Modules
{
    [Group("settings")]
    [RequireGuildRank(GuildRank.Admin)]
    [Name("Settings")]
    [Summary("Read and modify a guild specific variables.")]
    public class SettingsModule : BaseModule
    {
        public BotConfiguration BotConfig { get; set; } = null!;
        public UrfRidersDbContext DbContext { get; set; } = null!;

        private const string NullText = "*null*";
        private const string CustomPrefixKey = "prefix";
        private const string MemberRoleKey = "member";
        private const string ModeratorRoleKey = "moderator";
        private const string AdminRoleKey = "admin";
        
        private GuildSettings _settings = null!;

        protected override void BeforeExecute(CommandInfo command)
        {
            _settings = DbContext.GuildSettings.FindOrCreate(Context.Guild.Id);
        }

        [Command]
        [Priority(0)]
        [Name("Show all settings")]
        [Summary("A list of all available settings and their values.")]
        public async Task ShowAllSettings()
        {
            var embed = EmbedService
                .CreateBasic(title: "Settings")
                .WithAuthor(BotConfig.Name, Context.Client.CurrentUser.GetAvatarUrl())
                .WithDescription($"Current settings for {Context.Guild.Name}.")
                .AddField(f =>
                {
                    f.Name = "Member role";
                    f.Value = GetRoleMention(_settings.MemberRoleId) ?? NullText;
                    f.IsInline = true;
                })
                .AddField(f =>
                {
                    f.Name = "Moderator role";
                    f.Value = GetRoleMention(_settings.ModeratorRoleId) ?? NullText;
                    f.IsInline = true;
                })
                .AddField(f =>
                {
                    f.Name = "Admin role";
                    f.Value = GetRoleMention(_settings.AdminRoleId) ?? NullText;
                    f.IsInline = true;
                })
                .AddField(f =>
                {
                    f.Name = "Custom prefix";
                    f.Value = _settings.CustomPrefix.ToCode() ?? NullText;
                    f.IsInline = true;
                })
                .Build();

            await ReplyAsync(embed: embed);
        }

        [Command(CustomPrefixKey)]
        [Alias("customPrefix")]
        [Priority(1)]
        [Name("Set custom prefix")]
        [Summary("Choose a custom prefix for this server.")]
        public async Task SetCustomPrefix(string newPrefix)
        {
            _settings.CustomPrefix = newPrefix;
            await DbContext.SaveChangesAsync();

            var embed = EmbedService
                .CreateSuccess($"Prefix has been set to `{newPrefix}`.")
                .WithFooter("Tip: You can always invoke commands by mentioning me instead of using a prefix.")
                .Build();

            await ReplyAsync(embed: embed);
        }

        [Command(MemberRoleKey)]
        [Priority(1)]
        [Name("Set member role")]
        [Summary("Choose a role which will represent all members on this server.")]
        public async Task SetMemberRole(SocketRole role)
        {
            _settings.MemberRoleId = role.Id;
            await DbContext.SaveChangesAsync();

            var embed = EmbedService.CreateSuccess($"Member role has been set to {role.Mention}.").Build();
            await ReplyAsync(embed: embed);
        }

        [Command(ModeratorRoleKey)]
        [Priority(1)]
        [Name("Set moderator role")]
        [Summary("Choose a role which will represent all moderators on this server.")]
        public async Task SetModeratorRole(SocketRole role)
        {
            _settings.ModeratorRoleId = role.Id;
            await DbContext.SaveChangesAsync();

            var embed = EmbedService.CreateSuccess($"Moderator role has been set to {role.Mention}.").Build();
            await ReplyAsync(embed: embed);
        }

        [Command(AdminRoleKey)]
        [RequireGuildRank(GuildRank.Owner)]
        [Priority(1)]
        [Name("Set admin role")]
        [Summary("Choose a role which will represent all admin on this server.")]
        public async Task SetAdminRole(SocketRole role)
        {
            _settings.AdminRoleId = role.Id;
            await DbContext.SaveChangesAsync();

            var embed = EmbedService.CreateSuccess($"Admin role has been set to {role.Mention}.").Build();
            await ReplyAsync(embed: embed);
        }

        [Command("reset")]
        [Priority(1)]
        [Name("Reset a specific setting")]
        public async Task ResetSetting(string key)
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
                    await ReplyAsync(embed: EmbedService.CreateError("Setting not found.").Build());
                    return;
            }

            await DbContext.SaveChangesAsync();

            var embed = EmbedService.CreateSuccess($"{key} has been reset.").Build();
            await ReplyAsync(embed: embed);
        }

        [Command("resetAll")]
        [Priority(1)]
        [Name("Reset all settings for current server")]
        public async Task ResetAllSettings()
        {
            DbContext.Remove(_settings);
            await DbContext.SaveChangesAsync();

            var embed = EmbedService.CreateSuccess($"All settings for {Context.Guild.Name} has been reset.").Build();
            await ReplyAsync(embed: embed);
        }

        private string? GetRoleMention(ulong? roleId)
        {
            return roleId == null ? null : Context.Guild.GetRole(roleId.Value).Mention;
        }
        
    }
}