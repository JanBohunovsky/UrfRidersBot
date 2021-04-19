using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using LiteDB.Async;
using UrfRidersBot.Core.Common;
using UrfRidersBot.Core.ReactionRoles;
using UrfRidersBot.Infrastructure.Common;

namespace UrfRidersBot.Infrastructure.ReactionRoles
{
    internal class ReactionRoleRepository : LiteRepository<ReactionRoleDTO>, IReactionRoleRepository
    {
        private readonly DiscordClient _client;

        public ReactionRoleRepository(ILiteDatabaseAsync db, DiscordClient client) : base(db, "reaction_roles")
        {
            _client = client;
        }
        
        public async ValueTask<DiscordRole?> GetRoleAsync(DiscordMessage message, DiscordEmoji emoji)
        {
            var rawEmoji = emoji.ToString();
            var roleId = await Collection.Query()
                .Where(x => x.MessageId == message.Id && x.Emoji == rawEmoji)
                .Select(x => x.RoleId)
                .SingleOrDefaultAsync();

            if (roleId == default)
            {
                return null;
            }
            
            var guild = message.Channel.Guild;
            return guild.GetRole(roleId);
        }

        public async ValueTask<DiscordEmoji?> GetEmojiAsync(DiscordMessage message, DiscordRole role)
        {
            var rawEmoji = await Collection.Query()
                .Where(x => x.MessageId == message.Id && x.RoleId == role.Id)
                .Select(x => x.Emoji)
                .SingleOrDefaultAsync();

            if (rawEmoji == null)
            {
                return null;
            }

            return DiscordEmojiHelper.Parse(_client, rawEmoji);
        }

        public async ValueTask<bool> AddAsync(ReactionRole reactionRole)
        {
            var collection = Collection;
            await collection.EnsureIndexAsync(x => x.MessageId);

            var dto = ReactionRoleDTO.FromDiscord(reactionRole);
            
            // Message cannot have duplicate role or emoji
            var exists = await collection.ExistsAsync(x => x.MessageId == dto.MessageId
                                                           && (x.RoleId == dto.RoleId || x.Emoji == dto.Emoji));

            if (exists)
            {
                return false;
            }
            
            await collection.InsertAsync(dto);
            return true;
        }

        public async Task RemoveAsync(DiscordMessage message, DiscordRole role)
        {
            await Collection.DeleteManyAsync(x => x.MessageId == message.Id && x.RoleId == role.Id);
        }
    }
}