using System;
using DSharpPlus;
using DSharpPlus.Entities;
using LiteDB;
using UrfRidersBot.Core.Common;
using UrfRidersBot.Core.ReactionRoles;

namespace UrfRidersBot.Infrastructure.ReactionRoles
{
    internal class ReactionRoleDTO
    {
        public ObjectId Id { get; set; } = ObjectId.Empty;
        public ulong MessageId { get; set; }
        public ulong RoleId { get; set; }
        public string Emoji { get; set; } = "";

        public ReactionRole ToDiscord(DiscordClient client, DiscordMessage message)
        {
            var guild = message.Channel.Guild;
            var role = guild.GetRole(RoleId);
            var emoji = DiscordEmojiHelper.Parse(client, Emoji);

            if (emoji is null)
            {
                throw new InvalidOperationException("Could not parse emoji.");
            }

            return new ReactionRole(message, role, emoji);
        }

        public static ReactionRoleDTO FromDiscord(ReactionRole reactionRole)
        {
            return new ReactionRoleDTO
            {
                MessageId = reactionRole.Message.Id,
                RoleId = reactionRole.Role.Id,
                Emoji = reactionRole.Emoji.ToString()
            };
        }
    }
}