using Discord;
using UrfRiders.Util;

namespace UrfRiders.Data
{
    public class RoleData
    {
        public string EmoteText { get; set; }
        public ulong RoleId { get; set; }
        public ReactionRoleType Type { get; set; }

        public IEmote Emote => _emote ??= EmoteHelper.Parse(EmoteText);
        private IEmote _emote;

        public RoleData() { }

        public RoleData(string emoteText, ulong roleId, ReactionRoleType type)
        {
            EmoteText = emoteText;
            RoleId = roleId;
            Type = type;
        }

        public override string ToString() => $"{Emote} -> {RoleId} ({Type})";
    }
}