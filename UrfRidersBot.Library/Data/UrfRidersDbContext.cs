using Microsoft.EntityFrameworkCore;

namespace UrfRidersBot.Library
{
    public class UrfRidersDbContext : DbContext
    {
        public DbSet<GuildData> GuildData => Set<GuildData>();
        
        public UrfRidersDbContext(DbContextOptions<UrfRidersDbContext> options) : base(options)
        {
        }
    }
}