using System.Reflection;
using Microsoft.EntityFrameworkCore;
using UrfRidersBot.Core.Entities;
using UrfRidersBot.Persistence.DTOs;

namespace UrfRidersBot.Persistence
{
    public class UrfRidersDbContext : DbContext
    {
        public DbSet<AutoVoiceChannel> AutoVoiceChannels => Set<AutoVoiceChannel>();
        public DbSet<GuildSettings> GuildSettings => Set<GuildSettings>();
        public DbSet<ReactionRoleDTO> ReactionRoles => Set<ReactionRoleDTO>();
        internal DbSet<ColorRoleDTO> ColorRoles => Set<ColorRoleDTO>();
        
        public UrfRidersDbContext(DbContextOptions<UrfRidersDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}