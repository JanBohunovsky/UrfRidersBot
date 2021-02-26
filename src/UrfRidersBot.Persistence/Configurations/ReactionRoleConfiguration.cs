using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrfRidersBot.Persistence.DTOs;

namespace UrfRidersBot.Persistence.Configurations
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