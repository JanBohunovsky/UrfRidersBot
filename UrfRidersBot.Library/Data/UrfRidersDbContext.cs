using Microsoft.EntityFrameworkCore;

namespace UrfRidersBot.Library
{
    public class UrfRidersDbContext : DbContext
    {
        public DbSet<GuildSettings> GuildSettings => Set<GuildSettings>();
        
        public UrfRidersDbContext(DbContextOptions<UrfRidersDbContext> options) : base(options)
        {
        }
    }
}