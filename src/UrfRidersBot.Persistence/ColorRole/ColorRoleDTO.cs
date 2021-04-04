using DSharpPlus.Entities;

namespace UrfRidersBot.Persistence.ColorRole
{
    internal class ColorRoleDTO
    {
        public ulong GuildId { get; private set; }
        public ulong RoleId { get; private set; }
        public ulong UserId { get; private set; }

        public ColorRoleDTO(ulong guildId, ulong roleId, ulong userId)
        {
            GuildId = guildId;
            RoleId = roleId;
            UserId = userId;
        }

        public static ColorRoleDTO FromDiscord(DiscordRole role, DiscordMember member)
        {
            var guild = member.Guild;
            return new ColorRoleDTO(guild.Id, role.Id, member.Id);
        }
    }
}