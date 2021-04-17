using DSharpPlus.Entities;
using LiteDB;
using UrfRidersBot.Core.Settings;

namespace UrfRidersBot.Infrastructure.Settings
{
    internal class GuildSettingsDTO
    {
        [BsonId]
        public ulong GuildId { get; set; }
        public ulong? MemberRoleId { get; set; }
        public ulong? ModeratorRoleId { get; set; }
        public ulong? AdminRoleId { get; set; }

        public GuildSettings ToDiscord(DiscordGuild guild)
        {
            var result = new GuildSettings();

            if (MemberRoleId is not null)
            {
                result.MemberRole = guild.GetRole(MemberRoleId.Value);
            }
            
            if (ModeratorRoleId is not null)
            {
                result.ModeratorRole = guild.GetRole(ModeratorRoleId.Value);
            }
            
            if (AdminRoleId is not null)
            {
                result.AdminRole = guild.GetRole(AdminRoleId.Value);
            }

            return result;
        }

        public static GuildSettingsDTO FromDiscord(ulong guildId, GuildSettings settings)
        {
            return new()
            {
                GuildId = guildId,
                MemberRoleId = settings.MemberRole?.Id,
                ModeratorRoleId = settings.ModeratorRole?.Id,
                AdminRoleId = settings.AdminRole?.Id
            };
        }
    }
}