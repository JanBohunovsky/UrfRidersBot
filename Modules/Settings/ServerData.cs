using System.Collections.Generic;
using System.ComponentModel;
using UrfRiders.Modules.Covid19;
using UrfRiders.Util;

namespace UrfRiders.Modules.Settings
{
    public class ServerData
    {
        #region Core
#if DEBUG
        public string Prefix { get; set; } = "?";
#else
        public string Prefix { get; set; } = "!";
#endif
        #endregion

        #region Server Roles
        [Description("Admin can configure bot and can create and manage roles.")]
        public ulong? AdminRole { get; set; }

        [Description("Moderator can usually manage users and messages, and assign basic roles.")]
        public ulong? ModeratorRole { get; set; }

        [Description("Member is a verified user on the server.")]
        public ulong? MemberRole { get; set; }
        #endregion

        #region COVID-19 Module
        [Description("ID of text channel where COVID-19 Module posts updates.")]
        public ulong? Covid19Channel { get; set; }

        [Hidden]
        public Covid19Data Covid19CachedData { get; set; }
        #endregion

        #region Reaction Roles Module
        [Description("Where to look for message.\nUsed by Reaction Roles Module.")]
        public ulong? ReactionRolesChannel { get; set; }

        [Description("Selected message.\nUsed by Reaction Roles Module.")]
        public ulong? ReactionRolesMessage { get; set; }
        #endregion

        #region Auto Voice Module
        [Hidden]
        public List<ulong> AutoVoiceChannels { get; set; } = new List<ulong>();
        #endregion

        #region Clash Module
        [Description("ID of text channel where Clash Module posts upcoming tournaments and team builders.")]
        public ulong? ClashChannel { get; set; }
        #endregion

        #region Other
        [Description("Whether to use large code blocks in settings description.")]
        public bool LargeCodeBlock { get; set; }
        #endregion
    }
}