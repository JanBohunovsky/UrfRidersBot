using Microsoft.EntityFrameworkCore;

namespace UrfRidersBot
{
    public class UrfRidersDbContext : DbContext
    {
        public DbSet<GuildSettings> GuildSettings => Set<GuildSettings>();
        public DbSet<ReactionTrackerData> ReactionTrackerData => Set<ReactionTrackerData>();
        public DbSet<ReactionHandlerInfo> ActiveReactionHandlers => Set<ReactionHandlerInfo>();
        
        public UrfRidersDbContext(DbContextOptions<UrfRidersDbContext> options) : base(options)
        {
        }
    }
}