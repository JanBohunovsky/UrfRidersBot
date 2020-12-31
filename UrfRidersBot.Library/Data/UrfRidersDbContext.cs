using Microsoft.EntityFrameworkCore;

namespace UrfRidersBot.Library
{
    public class UrfRidersDbContext : DbContext
    {
        public DbSet<GuildSettings> GuildSettings => Set<GuildSettings>();
        public DbSet<ReactionTrackerData> ReactionTrackerData => Set<ReactionTrackerData>();
        
        internal DbSet<ReactionHandlerInfo> ActiveReactionHandlers => Set<ReactionHandlerInfo>();
        
        public UrfRidersDbContext(DbContextOptions<UrfRidersDbContext> options) : base(options)
        {
        }
    }
}