using DSharpPlus;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Entities;
using UrfRidersBot.Persistence.DTOs;

namespace UrfRidersBot.Persistence.Mappers
{
    public static class ReactionRoleMapper
    {
        public static ReactionRole ToDiscord(ReactionRoleDTO dto, DiscordClient client, DiscordMessage message)
        {
            var guild = message.Channel.Guild;
            var role = guild.GetRole(dto.RoleId);
            var emoji = EmojiMapper.ToDiscord(client, dto.Emoji);

            return new ReactionRole(message, emoji, role);
        }

        public static ReactionRoleDTO FromDiscord(ReactionRole reactionRole)
        {
            var emoji = EmojiMapper.FromDiscord(reactionRole.Emoji);
            return new ReactionRoleDTO(reactionRole.Message.Id, emoji, reactionRole.Role.Id);
        }
    }
}