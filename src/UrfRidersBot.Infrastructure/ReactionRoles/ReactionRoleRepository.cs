using DSharpPlus;
using DSharpPlus.Entities;
using LiteDB;
using UrfRidersBot.Core.ReactionRoles;
using UrfRidersBot.Infrastructure.Common;

namespace UrfRidersBot.Infrastructure.ReactionRoles
{
    internal class ReactionRoleRepository : LiteRepository<ReactionRoleDTO>, IReactionRoleRepository
    {
        private readonly DiscordClient _client;

        public ReactionRoleRepository(LiteDatabase db, DiscordClient client) : base(db, "reaction_roles")
        {
            _client = client;
        }
        
        public DiscordRole? GetRole(DiscordMessage message, DiscordEmoji emoji)
        {
            var rawEmoji = emoji.ToString();
            var roleId = Collection.Query()
                .Where(x => x.MessageId == message.Id && x.Emoji == rawEmoji)
                .Select(x => x.RoleId)
                .SingleOrDefault();

            if (roleId == default)
            {
                return null;
            }
            
            var guild = message.Channel.Guild;
            return guild.GetRole(roleId);
        }

        public DiscordEmoji? GetEmoji(DiscordMessage message, DiscordRole role)
        {
            var rawEmoji = Collection.Query()
                .Where(x => x.MessageId == message.Id && x.RoleId == role.Id)
                .Select(x => x.Emoji)
                .SingleOrDefault();

            if (rawEmoji == null)
            {
                return null;
            }

            return DiscordEmojiHelper.Parse(_client, rawEmoji);
        }

        public bool Add(ReactionRole reactionRole)
        {
            var collection = Collection;
            collection.EnsureIndex(x => x.MessageId);

            var dto = ReactionRoleDTO.FromDiscord(reactionRole);
            
            // Message cannot have duplicate role or emoji
            var exists = collection.Exists(x => x.MessageId == dto.MessageId
                                                && (x.RoleId == dto.RoleId || x.Emoji == dto.Emoji));

            if (exists)
            {
                return false;
            }
            
            collection.Insert(dto);
            return true;
        }

        public void Remove(DiscordMessage message, DiscordRole role)
        {
            Collection.DeleteMany(x => x.MessageId == message.Id && x.RoleId == role.Id);
        }
    }
}