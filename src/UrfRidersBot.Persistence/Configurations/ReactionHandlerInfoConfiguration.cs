using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrfRidersBot.Core.Entities;

namespace UrfRidersBot.Persistence.Configurations
{
    public class ReactionHandlerInfoConfiguration : IEntityTypeConfiguration<ReactionHandlerInfo>
    {
        public void Configure(EntityTypeBuilder<ReactionHandlerInfo> builder)
        {
            builder.HasKey(x => x.MessageId);
        }
    }
}