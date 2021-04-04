using System.Threading.Tasks;
using UrfRidersBot.Core.AutoVoice;
using UrfRidersBot.Core.ColorRole;
using UrfRidersBot.Core.Common;
using UrfRidersBot.Core.ReactionRoles;
using UrfRidersBot.Core.Settings;
using UrfRidersBot.Persistence.AutoVoice;
using UrfRidersBot.Persistence.ColorRole;
using UrfRidersBot.Persistence.ReactionRoles;
using UrfRidersBot.Persistence.Settings;

namespace UrfRidersBot.Persistence.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UrfRidersDbContext _context;

        public IAutoVoiceSettingsRepository AutoVoiceSettings { get; }
        public IGuildSettingsRepository GuildSettings { get; }
        public IReactionRoleRepository ReactionRoles { get; }
        public IColorRoleRepository ColorRoles { get; }

        public UnitOfWork(UrfRidersDbContext context)
        {
            _context = context;

            AutoVoiceSettings = new AutoVoiceSettingsRepository(_context);
            GuildSettings = new GuildSettingsRepository(_context);
            ReactionRoles = new ReactionRoleRepository(_context);
            ColorRoles = new ColorRoleRepository(_context);
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