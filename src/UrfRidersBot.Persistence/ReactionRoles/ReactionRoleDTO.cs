using DSharpPlus;
using DSharpPlus.Entities;
using UrfRidersBot.Core.ReactionRoles;
using UrfRidersBot.Persistence.Common;

namespace UrfRidersBot.Persistence.ReactionRoles
{
    public class ReactionRoleDTO
    {
        public ulong MessageId { get; private set; }
        public string Emoji { get; private set; }
        public ulong RoleId { get; private set; }

        public ReactionRoleDTO(ulong messageId, string emoji, ulong roleId)
        {
            MessageId = messageId;
            Emoji = emoji;
            RoleId = roleId;
        }
        public ReactionRole ToDiscord(DiscordClient client, DiscordMessage message)
        {
            var guild = message.Channel.Guild;
            var role = guild.GetRole(RoleId);
            var emoji = EmojiMapper.ToDiscord(client, Emoji);

            return new ReactionRole(message, emoji, role);
        }

        public static ReactionRoleDTO FromDiscord(ReactionRole reactionRole)
        {
            var emoji = EmojiMapper.FromDiscord(reactionRole.Emoji);
            return new ReactionRoleDTO(reactionRole.Message.Id, emoji, reactionRole.Role.Id);
        }
    }
}