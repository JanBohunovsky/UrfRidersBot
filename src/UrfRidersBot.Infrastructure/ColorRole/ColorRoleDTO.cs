using DSharpPlus.Entities;
using LiteDB;

namespace UrfRidersBot.Infrastructure.ColorRole
{
    internal class ColorRoleDTO
    {
        [BsonId]
        public ulong RoleId { get; set; }
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }

        public static ColorRoleDTO FromDiscord(DiscordRole role, DiscordGuild guild, DiscordUser user)
        {
            return new ColorRoleDTO
            {
                RoleId = role.Id,
                GuildId = guild.Id,
                UserId = user.Id,
            };
        }
    }
}