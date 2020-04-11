using Discord;
using Discord.Commands;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrfRiders.Data;

namespace UrfRiders.Interactive
{
    public class RoleHandler : IReactionHandler
    {
        public RunMode RunMode => RunMode.Sync;
        public bool HasRoles => Data.Count > 0;

        public List<RoleData> Data { get; private set; }

        public RoleHandler()
        {
            if (Data == null)
                Data = new List<RoleData>();
        }

        public bool AddRole(string emoteText, IRole role, ReactionRoleType type)
        {
            // Role and Emote must be unique
            if (Data.Any(r => r.RoleId == role.Id || r.EmoteText == emoteText))
                return false;

            Data.Add(new RoleData(emoteText, role.Id, type));
            return true;
        }

        public bool RemoveRole(IRole role)
        {
            return Data.RemoveAll(r => r.RoleId == role.Id) > 0;
        }

        public async Task ReactionAdded(IUserMessage message, IUser rawUser, IEmote emote)
        {
            var data = Data.FirstOrDefault(r => r.Emote.Equals(emote));
            if (data == null)
                return;
            if (!(rawUser is IGuildUser user))
                return;

            // normal -> add role and keep reaction
            // once   -> add role and remove reaction
            // remove -> remove role and remove reaction
            switch (data.Type)
            {
                case ReactionRoleType.Normal:
                    await AddRoleToUser(user, data.RoleId);
                    break;
                case ReactionRoleType.Once:
                    await AddRoleToUser(user, data.RoleId);
                    await message.RemoveReactionAsync(emote, user);
                    break;
                case ReactionRoleType.Remove:
                    await RemoveRoleFromUser(user, data.RoleId);
                    await message.RemoveReactionAsync(emote, user);
                    break;
            }
        }

        public async Task ReactionRemoved(IUserMessage message, IUser rawUser, IEmote emote)
        {
            var data = Data.FirstOrDefault(r => r.Emote.Equals(emote));
            if (data == null)
                return;
            if (!(rawUser is IGuildUser user))
                return;

            // normal -> remove role
            // once   -> nothing
            // remove -> nothing
            if (data.Type == ReactionRoleType.Normal)
                await RemoveRoleFromUser(user, data.RoleId);
        }

        private async Task AddRoleToUser(IGuildUser user, ulong roleId)
        {
            if (user.RoleIds.Contains(roleId))
                return;

            await user.AddRoleAsync(user.Guild.GetRole(roleId));
        }

        private async Task RemoveRoleFromUser(IGuildUser user, ulong roleId)
        {
            if (!user.RoleIds.Contains(roleId))
                return;

            await user.RemoveRoleAsync(user.Guild.GetRole(roleId));
        }
    }
}