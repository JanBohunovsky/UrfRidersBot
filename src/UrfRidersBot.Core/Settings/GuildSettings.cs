using DSharpPlus.Entities;

namespace UrfRidersBot.Core.Settings
{
    public class GuildSettings
    {
        public ulong GuildId { get; set; }
        public ulong? MemberRoleId { get; set; }
        public ulong? ModeratorRoleId { get; set; }
        public ulong? AdminRoleId { get; set; }

        public GuildSettings(ulong guildId)
        {
            GuildId = guildId;
        }

        public DiscordRole? GetMemberRole(DiscordGuild guild)
        {
            if (MemberRoleId is null)
            {
                return null;
            }

            return guild.GetRole(MemberRoleId.Value);
        }

        public DiscordRole? GetModeratorRole(DiscordGuild guild)
        {
            if (ModeratorRoleId is null)
            {
                return null;
            }

            return guild.GetRole(ModeratorRoleId.Value);
        }

        public DiscordRole? GetAdminRole(DiscordGuild guild)
        {
            if (AdminRoleId is null)
            {
                return null;
            }

            return guild.GetRole(AdminRoleId.Value);
        }
    }
}