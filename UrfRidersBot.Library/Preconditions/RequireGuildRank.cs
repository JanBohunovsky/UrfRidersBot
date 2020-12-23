using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace UrfRidersBot.Library
{
    /// <summary>
    /// Requires the user invoking the command to have this guild rank or higher.
    /// <para>
    /// Example: If you select <see cref="GuildRank.Member"/> then users that are at least that rank
    /// or higher (e.g. <see cref="GuildRank.Moderator"/>) can invoke this command.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class RequireGuildRank : PreconditionAttribute
    {
        private static readonly Task<PreconditionResult> NotGuild =
            Task.FromResult(PreconditionResult.FromError("You must be in a guild to run this command."));

        private static readonly Task<PreconditionResult> NotOwner =
            Task.FromResult(PreconditionResult.FromError("Only the guild owner is permitted to run this command."));
        
        private static Task<PreconditionResult> NotPermitted(string missingRank) =>
            Task.FromResult(PreconditionResult.FromError($"You must be {missingRank} to run this command."));

        private static readonly Task<PreconditionResult> Permitted =
            Task.FromResult(PreconditionResult.FromSuccess());
        
        private readonly GuildRank _rank;
        
        public RequireGuildRank(GuildRank rank) => _rank = rank;

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            // Only valid in guild.
            if (!(context.User is SocketGuildUser user))
                return NotGuild;

            // TODO: Get guild settings
            var guildSettings = new object();

            // Check user's rank
            var userRank = GetUserRank(user, guildSettings);
            if (userRank >= _rank)
                return Permitted;
            
            // Not permitted - find out the best error message.
            if (_rank == GuildRank.Owner)
            {
                return NotOwner;
            }
            else
            {
                var missingRank = GetRankName(_rank, context.Guild, guildSettings);
                return NotPermitted(missingRank);
            }
        }

        private static GuildRank GetUserRank(SocketGuildUser user, object guildSettings)
        {
            var rank = GuildRank.Everyone;
            if (UserHasRole(user, null) ?? true)
                rank = GuildRank.Member;
            if (UserHasRole(user, null) ?? user.GuildPermissions.ManageChannels)
                rank = GuildRank.Moderator;
            if (UserHasRole(user, null) ?? user.GuildPermissions.Administrator)
                rank = GuildRank.Admin;
            if (user.Guild.OwnerId == user.Id)
                rank = GuildRank.Owner;

            return rank;
        }

        private static string GetRankName(GuildRank rank, IGuild guild, object guildSettings)
        {
            return rank switch
            {
                GuildRank.Everyone   => "everyone",
                GuildRank.Member     => GetRoleMention(guild, null) ?? "a member",
                GuildRank.Moderator  => GetRoleMention(guild, null) ?? "a moderator",
                GuildRank.Admin      => GetRoleMention(guild, null) ?? "an admin",
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