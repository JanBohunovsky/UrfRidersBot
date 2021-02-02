﻿using System.ComponentModel.DataAnnotations;

namespace UrfRidersBot
{
    public class GuildSettings
    {
        [Key]
        public ulong GuildId { get; set; }
        
        public string? CustomPrefix { get; set; }
        public ulong? MemberRoleId { get; set; }
        public ulong? ModeratorRoleId { get; set; }
        public ulong? AdminRoleId { get; set; }

        public GuildSettings(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}