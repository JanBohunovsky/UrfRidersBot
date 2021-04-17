using DSharpPlus.Entities;

namespace UrfRidersBot.Core.Settings
{
    public class GuildSettings
    {
        public DiscordRole? MemberRole { get; set; }
        public DiscordRole? ModeratorRole { get; set; }
        public DiscordRole? AdminRole { get; set; }
    }
}