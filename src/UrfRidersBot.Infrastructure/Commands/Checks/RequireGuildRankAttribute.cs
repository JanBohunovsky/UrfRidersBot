using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Settings;

namespace UrfRidersBot.Infrastructure.Commands.Checks
{
    public class RequireGuildRankAttribute : CheckAttribute
    {
        private readonly GuildRank _rank;
        
        public RequireGuildRankAttribute(GuildRank rank) => _rank = rank;
        
        public override async ValueTask<CheckResult> CheckAsync(ICommandContext context, IServiceProvider provider)
        {
            // Get guild settings
            using var repository = provider.GetRequiredService<IGuildSettingsRepository>();
            var guildSettings = await repository.GetOrCreateAsync();
            
            // Check member's rank
            var memberRank = GetMemberRank(context.Member, context.Channel, guildSettings);
            return memberRank >= _rank
                ? CheckResult.Successful
                : CheckResult.Unsuccessful($"You must be {_rank.ToString()} to execute this command.");
        }

        /// <summary>
        /// Figures out member's highest guild rank.
        /// </summary>
        private static GuildRank GetMemberRank(DiscordMember member, DiscordChannel channel, GuildSettings guildSettings)
        {
            var rank = GuildRank.Everyone;
            if (MemberHasRole(member, guildSettings.MemberRole) ?? true)
                rank = GuildRank.Member;
            if (MemberHasRole(member, guildSettings.ModeratorRole) ??
                member.PermissionsIn(channel).HasPermission(Permissions.ManageChannels))
                rank = GuildRank.Moderator;
            if (MemberHasRole(member, guildSettings.AdminRole) ??
                member.PermissionsIn(channel).HasPermission(Permissions.Administrator))
                rank = GuildRank.Admin;
            if (member.Guild.OwnerId == member.Id)
                rank = GuildRank.Owner;

            return rank;
        }
        
        /// <summary>
        /// Checks if user has a role. If <see cref="role"/> is null then this method also returns null.
        /// <para>
        /// This is a little helper method so I don't have to check if <see cref="role"/> is null, or I don't have to use it twice.
        /// With this I can use the null coalescing operator (the double question mark).
        /// </para>
        /// </summary>
        private static bool? MemberHasRole(DiscordMember member, DiscordRole? role)
        {
            if (role == null)
                return null;
            
            return member.Roles.Contains(role);
        }
    }
}