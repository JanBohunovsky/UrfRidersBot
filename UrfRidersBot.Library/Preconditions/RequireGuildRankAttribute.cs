using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace UrfRidersBot.Library
{
    /// <summary>
    /// Requires the user invoking the command to have this guild rank or higher.
    /// This precondition automatically applies <see cref="RequireContextAttribute"/>.
    /// <para>
    /// Example: If you select <see cref="GuildRank.Member"/> then users that are at least that rank
    /// or higher (e.g. <see cref="GuildRank.Moderator"/>) can invoke this command.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class RequireGuildRankAttribute : RequireContextAttribute
    {
        private readonly GuildRank _rank;
        
        public RequireGuildRankAttribute(GuildRank rank) : base(ContextType.Guild) => _rank = rank;

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            // Check if this is invoked in a guild
            var baseResult = await base.CheckPermissionsAsync(context, command, provider);
            if (!baseResult.IsSuccess)
                return baseResult;

            // Get guild user. We already checked if the context is in guild so this should never fail.
            var user = (SocketGuildUser)context.User;

            // Get guild settings
            var dbContextFactory = provider.GetRequiredService<IDbContextFactory<UrfRidersDbContext>>();
            await using var dbContext = dbContextFactory.CreateDbContext();
            var guildSettings = await dbContext.GuildSettings.FindOrCreateAsync(context.Guild.Id);

            // Check user's rank
            var userRank = GetUserRank(user, guildSettings);
            if (userRank >= _rank)
                return PreconditionResult.FromSuccess();
            
            // Not permitted - find out the best error message.
            if (_rank == GuildRank.Owner)
            {
                return PreconditionResult.FromError("Only the guild owner is permitted to run this command.");
            }
            else
            {
                var missingRank = GetRankName(_rank, context.Guild, guildSettings);
                return PreconditionResult.FromError($"You must be {missingRank} to run this command.");
            }
        }

        private static GuildRank GetUserRank(SocketGuildUser user, GuildSettings guildSettings)
        {
            var rank = GuildRank.Everyone;
            if (UserHasRole(user, guildSettings.MemberRoleId) ?? true)
                rank = GuildRank.Member;
            if (UserHasRole(user, guildSettings.ModeratorRoleId) ?? user.GuildPermissions.ManageChannels)
                rank = GuildRank.Moderator;
            if (UserHasRole(user, guildSettings.AdminRoleId) ?? user.GuildPermissions.Administrator)
                rank = GuildRank.Admin;
            if (user.Guild.OwnerId == user.Id)
                rank = GuildRank.Owner;

            return rank;
        }

        private static string GetRankName(GuildRank rank, IGuild guild, GuildSettings guildSettings)
        {
            return rank switch
            {
                GuildRank.Everyone   => "everyone",
                GuildRank.Member     => GetRoleMention(guild, guildSettings.MemberRoleId) ?? "a member",
                GuildRank.Moderator  => GetRoleMention(guild, guildSettings.ModeratorRoleId) ?? "a moderator",
                GuildRank.Admin      => GetRoleMention(guild, guildSettings.AdminRoleId) ?? "an admin",
                GuildRank.Owner      => "the guild owner",
                _                    => throw new ArgumentOutOfRangeException(nameof(rank))
            };
        }

        /// <summary>
        /// Checks if user has a role. If <see cref="roleId"/> is null then this method also returns null.
        /// <para>
        /// This is a little helper method so I don't have to check if <see cref="roleId"/> is null, or I don't have to use it twice.
        /// With this I can use the null coalescing operator (the double question mark).
        /// </para>
        /// </summary>
        private static bool? UserHasRole(SocketGuildUser user, ulong? roleId)
        {
            if (roleId == null)
                return null;
            
            return user.Roles.Any(r => r.Id == roleId);
        }

        private static string? GetRoleMention(IGuild guild, ulong? roleId)
        {
            if (roleId == null)
                return null;
            
            return guild.GetRole(roleId.Value)?.Mention;
        }
    }
}