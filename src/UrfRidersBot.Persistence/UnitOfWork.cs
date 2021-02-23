using System.Threading.Tasks;
using UrfRidersBot.Core.Interfaces;
using UrfRidersBot.Persistence.Repositories;

namespace UrfRidersBot.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UrfRidersDbContext _context;

        public IAutoVoiceChannelRepository AutoVoiceChannels { get; }
        public IGuildSettingsRepository GuildSettings { get; set; }

        public UnitOfWork(UrfRidersDbContext context)
        {
            _context = context;

            AutoVoiceChannels = new AutoVoiceChannelRepository(_context);
            GuildSettings = new GuildSettingsRepository(_context);
        }
        
        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}