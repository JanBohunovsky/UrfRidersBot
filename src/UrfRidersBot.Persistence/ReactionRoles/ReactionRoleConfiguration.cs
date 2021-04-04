using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UrfRidersBot.Persistence.ReactionRoles
{
    public class ReactionRoleConfiguration : IEntityTypeConfiguration<ReactionRoleDTO>
    {
        public void Configure(EntityTypeBuilder<ReactionRoleDTO> builder)
        {
            builder.HasKey(x => new { x.MessageId, x.Emoji, x.RoleId });
            
            builder.HasIndex(x => new { x.MessageId, x.Emoji }).IsUnique();
            builder.HasIndex(x => new { x.MessageId, x.RoleId }).IsUnique();
        }
    }
}