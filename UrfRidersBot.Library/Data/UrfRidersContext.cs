using Microsoft.EntityFrameworkCore;

namespace UrfRidersBot.Library
{
    public class UrfRidersContext : DbContext
    {
        public DbSet<GuildData> GuildData => Set<GuildData>();
        
        public UrfRidersContext(DbContextOptions<UrfRidersContext> options) : base(options)
        {
        }
    }
}