using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using UrfRidersBot.Core.Entities;
using UrfRidersBot.Core.Interfaces;
using UrfRidersBot.Persistence.Mappers;

namespace UrfRidersBot.Persistence.Repositories
{
    public class ReactionRoleRepository : IReactionRoleRepository
    {
        private readonly UrfRidersDbContext _context;

        public ReactionRoleRepository(UrfRidersDbContext context)
        {
            _context = context;
        }
        
        public async ValueTask<DiscordRole?> GetRoleAsync(DiscordMessage message, DiscordEmoji emoji)
        {
            var rawEmoji = EmojiMapper.FromDiscord(emoji);
            var roleId = await _context.ReactionRoles
                .Where(x => x.MessageId == message.Id && x.Emoji == rawEmoji)
                .Select(x => x.RoleId)
                .SingleOrDefaultAsync();

            if (roleId == default)
                return null;
            
            var guild = message.Channel.Guild;
            return guild.GetRole(roleId);
        }

        public async ValueTask<DiscordEmoji?> GetEmojiAsync(DiscordClient client, DiscordMessage message, DiscordRole role)
        {
            var rawEmoji = await _context.ReactionRoles
                .Where(x => x.MessageId == message.Id && x.RoleId == role.Id)
                .Select(x => x.Emoji)
                .SingleOrDefaultAsync();

            if (rawEmoji == null)
                return null;

            return EmojiMapper.ToDiscord(client, rawEmoji);
        }

        public async Task AddAsync(ReactionRole reactionRole)
        {
            await _context.ReactionRoles.AddAsync(ReactionRoleMapper.FromDiscord(reactionRole));
        }

        public void Remove(ReactionRole reactionRole)
        {
            _context.ReactionRoles.Remove(ReactionRoleMapper.FromDiscord(reactionRole));
        }
    }
}