using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Commands.Attributes;
using UrfRidersBot.Core.Entities;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Infrastructure.Commands.Checks
{
    public class RequireGuildRankAttribute : CheckAttribute
    {
        private readonly GuildRank _rank;
        
        public RequireGuildRankAttribute(GuildRank rank) => _rank = rank;
        
        public override async ValueTask<CheckResult> CheckAsync(CommandContext context, IServiceProvider provider)
        {
            if (context.Guild is null || context.Member is null)
            {
                return CheckResult.Unsuccessful("This command can be used only in a server.");
            }
            
            // Get guild settings
            var unitOfWorkFactory = provider.GetRequiredService<IUnitOfWorkFactory>();
            await using var unitOfWork = unitOfWorkFactory.Create();
            var guildSettings = await unitOfWork.GuildSettings.GetOrCreateAsync(context.Guild);
            
            // Check member's rank
            var memberRank = GetMemberRank(context.Member, context.Channel, guildSettings);
            return memberRank >= _rank
                ? CheckResult.Successful
                : CheckResult.Unsuccessful($"You must be at least {_rank.ToString()} to execute this command.");
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