using System.Reflection;
using Microsoft.EntityFrameworkCore;
using UrfRidersBot.Core.Entities;

namespace UrfRidersBot.Persistence
{
    public class UrfRidersDbContext : DbContext
    {
        public DbSet<AutoVoiceChannel> AutoVoiceChannels => Set<AutoVoiceChannel>();
        public DbSet<GuildSettings> GuildSettings => Set<GuildSettings>();
        
        // Old stuff
        public DbSet<ReactionTrackerData> ReactionTrackerData => Set<ReactionTrackerData>();
        public DbSet<ReactionHandlerInfo> ActiveReactionHandlers => Set<ReactionHandlerInfo>();
        
        public UrfRidersDbContext(DbContextOptions<UrfRidersDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}