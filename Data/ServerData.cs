using System.Collections.Generic;
using System.ComponentModel;
using UrfRiders.Attributes;
using UrfRiders.Interactive;

namespace UrfRiders.Data
{
    public class ServerData
    {
#if DEBUG
        public string Prefix { get; set; } = "?";
#else
        public string Prefix { get; set; } = "!";
#endif
        [Description("Admin can configure bot and can create and manage roles.")]
        public ulong? AdminRole { get; set; }

        [Description("Moderator can usually manage users and messages, and assign basic roles.")]
        public ulong? ModeratorRole { get; set; }

        [Description("Member is a verified user on the server.")]
        public ulong? MemberRole { get; set; }

        [Description("ID of text channel where COVID-19 Module posts updates.")]
        public ulong? Covid19Channel { get; set; }

        [Description("Where to look for message.\nUsed by Reaction Roles Module.")]
        public ulong? ReactionRolesChannel { get; set; }

        [Description("Selected message.\nUsed by Reaction Roles Module.")]
        public ulong? ReactionRolesMessage { get; set; }

        [Description("Whether to use large code blocks in settings description.")]
        public bool LargeCodeBlock { get; set; }

        [Hidden]
        public List<ulong> AutoVoiceChannels { get; set; } = new List<ulong>();
    }
}