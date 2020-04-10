using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UrfRiders.Data;
using UrfRiders.Services;

namespace UrfRiders.Attributes.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RequireLevelAttribute : PreconditionAttribute
    {
        private const string MissingRole = "You must be {0} to use this command.";
        private const string NotOwner = "Only the server owner is allowed to use this command.";

        private readonly PermissionLevel _level;

        public RequireLevelAttribute(PermissionLevel level) => _level = level;

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            // Only valid in guild
            if (!(context.User is SocketGuildUser user))
                return Task.FromResult(PreconditionResult.FromError("You must be in a guild to run this command."));

            // Everyone has access
            if (_level == PermissionLevel.Everyone)
                return Task.FromResult(PreconditionResult.FromSuccess());

            // Owner can do anything, thus we don't need to check for other levels
            if (context.Guild.OwnerId == user.Id)
                return Task.FromResult(PreconditionResult.FromSuccess());

            var logger = services.GetRequiredService<ILogger<RequireLevelAttribute>>();
            var database = services.GetRequiredService<LiteDatabase>();
            var settings = new ServerSettings(context.Guild.Id, database);

            // Check member
            if (_level == PermissionLevel.Member)
            {
                // If server didn't set member role, then we assume that 'Member' is the same level as `Everyone`
                if (!settings.MemberRole.HasValue)
                {
                    //logger.LogInformation($"No member role set for guild {context.Guild.Name} ({context.Guild.Id})! Assuming Member=Everyone");
                    return Task.FromResult(PreconditionResult.FromSuccess());
                }

                var memberRole = context.Guild.GetRole(settings.MemberRole.Value);
                if (user.Roles.Any(r => r.Id == settings.MemberRole.Value))
                    return Task.FromResult(PreconditionResult.FromSuccess());
                else
                    return Task.FromResult(PreconditionResult.FromError(string.Format(MissingRole, memberRole.Mention)));
            }

            // Check moderator
            if (_level == PermissionLevel.Moderator)
            {
                // If server didn't set moderator role, then we assume that moderators have 'Manage Channels' permission.
                if (!settings.ModeratorRole.HasValue)
                {
                    //logger.LogInformation($"No moderator role set for guild {context.Guild.Name} ({context.Guild.Id})! Assuming ManageChannels=Moderator");
                    if (user.GuildPermissions.Has(GuildPermission.ManageChannels))
                        return Task.FromResult(PreconditionResult.FromSuccess());
                    else
                        return Task.FromResult(PreconditionResult.FromError(string.Format(MissingRole, "a moderator")));
                }

                var moderatorRole = context.Guild.GetRole(settings.ModeratorRole.Value);
                if (user.Roles.Any(r => r.Id == settings.ModeratorRole.Value))
                    return Task.FromResult(PreconditionResult.FromSuccess());
                else
                    return Task.FromResult(PreconditionResult.FromError(string.Format(MissingRole, moderatorRole.Mention)));
            }

            // Check admin
            if (_level == PermissionLevel.Admin)
            {
                // If server didn't set admin role, then we assume that `Admin` is the same level as `Owner`
                if (!settings.AdminRole.HasValue)
                {
                    //logger.LogInformation($"No admin role set for guild {context.Guild.Name} ({context.Guild.Id})! Assuming Admin=Owner");
                    // Since we already checked for owner, only possible result is failure
                    return Task.FromResult(PreconditionResult.FromError(string.Format(MissingRole, "an admin")));
                }

                var adminRole = context.Guild.GetRole(settings.AdminRole.Value);
                if (user.Roles.Any(r => r.Id == settings.AdminRole.Value))
                    return Task.FromResult(PreconditionResult.FromSuccess());
                else
                    return Task.FromResult(PreconditionResult.FromError(string.Format(MissingRole, adminRole.Mention)));
            }

            // Check owner
            if (_level == PermissionLevel.Owner)
            {
                // We already checked if user is owner
                return Task.FromResult(PreconditionResult.FromError(NotOwner));
            }

            // Shouldn't happen
            logger.LogWarning($"End of RequireLevel Check, probably invalid PermissionLevel: '{_level}'");
            return Task.FromResult(PreconditionResult.FromError("Internal error"));
        }
    }
}