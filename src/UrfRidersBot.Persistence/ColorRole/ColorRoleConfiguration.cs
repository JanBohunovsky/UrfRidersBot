using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UrfRidersBot.Persistence.ColorRole
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