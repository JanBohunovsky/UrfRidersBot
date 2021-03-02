using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrfRidersBot.Persistence.DTOs;

namespace UrfRidersBot.Persistence.Configurations
{
    internal class ColorRoleConfiguration : IEntityTypeConfiguration<ColorRoleDTO>
    {
        public void Configure(EntityTypeBuilder<ColorRoleDTO> builder)
        {
            builder.HasKey(x => new { x.GuildId, x.RoleId, x.UserId });

            builder.HasIndex(x => new { x.GuildId, x.RoleId }).IsUnique();
            builder.HasIndex(x => new { x.GuildId, x.UserId }).IsUnique();
        }
    }
}