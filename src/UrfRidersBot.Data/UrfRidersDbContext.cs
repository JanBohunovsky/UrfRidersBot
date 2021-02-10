using Microsoft.EntityFrameworkCore;

namespace UrfRidersBot.Data
{
    public class UrfRidersDbContext : DbContext
    {
        public DbSet<GuildSettings> GuildSettings => Set<GuildSettings>();
        public DbSet<AutoVoiceChannel> AutoVoiceChannels => Set<AutoVoiceChannel>();
        public DbSet<ReactionTrackerData> ReactionTrackerData => Set<ReactionTrackerData>();
        public DbSet<ReactionHandlerInfo> ActiveReactionHandlers => Set<ReactionHandlerInfo>();
        
        public UrfRidersDbContext(DbContextOptions<UrfRidersDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GuildSettings>().HasKey(x => x.GuildId);
            modelBuilder.Entity<AutoVoiceChannel>().HasKey(x => new { x.GuildId, x.VoiceChannelId });
            modelBuilder.Entity<ReactionTrackerData>().HasKey(x => x.MessageId);
            modelBuilder.Entity<ReactionHandlerInfo>().HasKey(x => x.MessageId);
        }
    }
}