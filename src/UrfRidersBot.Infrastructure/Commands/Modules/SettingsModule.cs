using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using UrfRidersBot.Core;
using UrfRidersBot.Core.Entities;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Infrastructure.Commands.Modules
{
    [Group("settings")]
    [RequireGuildRank(GuildRank.Admin)]
    [Description("Read and modify a server specific variables.")]
    [ModuleLifespan(ModuleLifespan.Transient)]
    public class SettingsModule : BaseCommandModule
    {
        private const string NullText = "*null*";
        private const string CustomPrefixKey = "prefix";
        private const string MemberRoleKey = "member";
        private const string ModeratorRoleKey = "moderator";
        private const string AdminRoleKey = "admin";
        private const string BitrateKey = "bitrate";

        private readonly IUnitOfWork _unitOfWork;
        private GuildSettings _settings = null!;

        public SettingsModule(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public override async Task BeforeExecutionAsync(CommandContext ctx)
        {
            _settings = await _unitOfWork.GuildSettings.GetOrCreateAsync(ctx.Guild);
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
                    _settings.CustomPrefix != null ? Markdown.Code(_settings.CustomPrefix) : NullText,
                    true)
                .AddField(
                    "Bitrate",
                    _settings.VoiceBitrate != null ? $"{_settings.VoiceBitrate} Kbps" : NullText,
                    true);
            
            await ctx.RespondAsync(embed);
        }

        [Command(CustomPrefixKey)]
        [Aliases("customPrefix")]
        [Description("Choose a custom prefix for this server.")]
        public async Task SetCustomPrefix(CommandContext ctx, [Description("Prefix to use on this server.")] string newPrefix)
        {
            _settings.CustomPrefix = newPrefix;
            await _unitOfWork.CompleteAsync();

            var embed = EmbedHelper
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
            
            // TODO: Consider moving this to AfterExecutionAsync()
            // All commands except the group command use this method
            await _unitOfWork.CompleteAsync();

            var embed = EmbedHelper.CreateSuccess($"Member role has been set to {role.Mention}.").Build();
            await ctx.RespondAsync(embed);
        }

        [Command(ModeratorRoleKey)]
        [Description("Choose a role which will represent all moderators on this server.")]
        public async Task SetModeratorRole(CommandContext ctx, [Description("Role that describes a server moderator.")] DiscordRole role)
        {
            _settings.ModeratorRoleId = role.Id;
            await _unitOfWork.CompleteAsync();

            var embed = EmbedHelper.CreateSuccess($"Moderator role has been set to {role.Mention}.").Build();
            await ctx.RespondAsync(embed);
        }

        [Command(AdminRoleKey)]
        [Description("Choose a role which will represent all admin on this server.")]
        public async Task SetAdminRole(CommandContext ctx, [Description("Role that describes a server admin.")] DiscordRole role)
        {
            _settings.AdminRoleId = role.Id;
            await _unitOfWork.CompleteAsync();

            var embed = EmbedHelper.CreateSuccess($"Admin role has been set to {role.Mention}.").Build();
            await ctx.RespondAsync(embed);
        }

        [Command(BitrateKey)]
        [Description("Set bitrate for auto voice channels.")]
        public async Task SetBitrate(CommandContext ctx, int bitrate)
        {
            const int minBitrate = 8;
            var maxBitrate = ctx.Guild.PremiumTier switch
            {
                PremiumTier.Tier_1  => 128,
                PremiumTier.Tier_2  => 256,
                PremiumTier.Tier_3  => 384,
                PremiumTier.Unknown => 384, // Handle unknown as a new tier above 3
                _ => 96
            };

            if (bitrate < minBitrate || bitrate > maxBitrate)
            {
                await ctx.RespondAsync(EmbedHelper.CreateError($"Bitrate must be between {minBitrate} and {maxBitrate} Kbps."));
                return;
            }

            _settings.VoiceBitrate = bitrate;
            await _unitOfWork.CompleteAsync();

            var embed = EmbedHelper
                .CreateSuccess($"Auto Voice channel's bitrate has been set to {bitrate} Kbps.")
                .WithFooter("Existing Auto Voice channels won't be updated.");

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
                case BitrateKey:
                    _settings.VoiceBitrate = null;
                    key = "Bitrate";
                    break;
                default:
                    await ctx.RespondAsync(EmbedHelper.CreateError("Setting not found.").Build());
                    return;
            }

            await _unitOfWork.CompleteAsync();

            var embed = EmbedHelper.CreateSuccess($"{key} has been reset.").Build();
            await ctx.RespondAsync(embed);
        }

        [Command("resetAll")]
        [Description("Reset all settings for current server")]
        public async Task ResetAllSettings(CommandContext ctx)
        {
            _unitOfWork.GuildSettings.Remove(_settings);
            await _unitOfWork.CompleteAsync();

            var embed = EmbedHelper.CreateSuccess($"All settings for {ctx.Guild.Name} has been reset.").Build();
            await ctx.RespondAsync(embed);
        }

        // TODO: Move this to GuildSettings
        private string? GetRoleMention(CommandContext ctx, ulong? roleId)
        {
            return roleId == null ? null : ctx.Guild.GetRole(roleId.Value).Mention;
        }
    }
}