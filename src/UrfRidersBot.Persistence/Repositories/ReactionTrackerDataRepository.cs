using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Entities;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Persistence.Repositories
{
    public class ReactionTrackerDataRepository : IReactionTrackerDataRepository
    {
        private readonly UrfRidersDbContext _context;

        public ReactionTrackerDataRepository(UrfRidersDbContext context)
        {
            _context = context;
        }
        
        public async Task AddAsync(ReactionTrackerData data)
        {
            await _context.ReactionTrackerData.AddAsync(data);
        }

        public async ValueTask<ReactionTrackerData?> GetByMessageAsync(DiscordMessage message)
        {
            return await _context.ReactionTrackerData.FindAsync(message.Id);
        }

        public void Remove(ReactionTrackerData data)
        {
            _context.ReactionTrackerData.Remove(data);
        }
    }
}