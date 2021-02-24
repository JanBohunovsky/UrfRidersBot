using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UrfRidersBot.Core.Entities;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Persistence.Repositories
{
    public class ReactionHandlerInfoRepository : IReactionHandlerInfoRepository
    {
        private readonly UrfRidersDbContext _context;

        public ReactionHandlerInfoRepository(UrfRidersDbContext context)
        {
            _context = context;
        }

        public async ValueTask<IEnumerable<ReactionHandlerInfo>> GetAllAsync()
        {
            return await _context.ActiveReactionHandlers.ToListAsync();
        }

        public async ValueTask<ReactionHandlerInfo?> GetByMessageId(ulong messageId)
        {
            return await _context.ActiveReactionHandlers.FindAsync(messageId);
        }

        public async Task AddAsync(ReactionHandlerInfo handlerInfo)
        {
            await _context.ActiveReactionHandlers.AddAsync(handlerInfo);
        }

        public void Remove(ReactionHandlerInfo handlerInfo)
        {
            _context.Remove(handlerInfo);
        }
    }
}