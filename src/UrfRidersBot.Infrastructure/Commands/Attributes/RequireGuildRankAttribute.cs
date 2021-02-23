using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using UrfRidersBot.Core.Entities;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Infrastructure.Commands
{
    /// <summary>
    /// Requires the user invoking the command to have this guild rank or higher.
    /// <para>
    /// Example: If you select <see cref="GuildRank.Member"/> then users that are at least that rank
    /// or higher (e.g. <see cref="GuildRank.Moderator"/>) can invoke this command.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class RequireGuildRankAttribute : CheckBaseAttribute
    {
        private readonly GuildRank _rank;
        
        public RequireGuildRankAttribute(GuildRank rank) => _rank = rank;

        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            // Check if this is invoked in a guild
            if (ctx.Guild == null)
                return false;

            var member = (DiscordMember)ctx.User;
            
            // Get guild settings
            var unitOfWorkFactory = ctx.Services.GetRequiredService<IUnitOfWorkFactory>();
            await using var unitOfWork = unitOfWorkFactory.Create();
            var guildSettings = await unitOfWork.GuildSettings.GetOrCreateAsync(ctx.Guild);
            
            // Check user's rank
            var memberRank = GetMemberRank(member, ctx.Channel, guildSettings);
            return memberRank >= _rank;
        }

        /// <summary>
        /// Figures out member's highest guild rank.
        /// </summary>
        private static GuildRank GetMemberRank(DiscordMember member, DiscordChannel channel, GuildSettings guildSettings)
        {
            var rank = GuildRank.Everyone;
            if (MemberHasRole(member, guildSettings.MemberRoleId) ?? true)
                rank = GuildRank.Member;
            if (MemberHasRole(member, guildSettings.ModeratorRoleId) ??
                member.PermissionsIn(channel).HasPermission(Permissions.ManageChannels))
                rank = GuildRank.Moderator;
            if (MemberHasRole(member, guildSettings.AdminRoleId) ??
                member.PermissionsIn(channel).HasPermission(Permissions.Administrator))
                rank = GuildRank.Admin;
            if (member.Guild.OwnerId == member.Id)
                rank = GuildRank.Owner;

            return rank;
        }

        /// <summary>
        /// Checks if user has a role. If <see cref="roleId"/> is null then this method also returns null.
        /// <para>
        /// This is a little helper method so I don't have to check if <see cref="roleId"/> is null, or I don't have to use it twice.
        /// With this I can use the null coalescing operator (the double question mark).
        /// </para>
        /// </summary>
        private static bool? MemberHasRole(DiscordMember member, ulong? roleId)
        {
            if (roleId == null)
                return null;
            
            return member.Roles.Any(r => r.Id == roleId);
        }
    }
}