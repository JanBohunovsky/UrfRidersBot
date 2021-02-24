using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrfRidersBot.Core.Entities;

namespace UrfRidersBot.Persistence.Configurations
{
    public class ReactionTrackerDataConfiguration : IEntityTypeConfiguration<ReactionTrackerData>
    {
        public void Configure(EntityTypeBuilder<ReactionTrackerData> builder)
        {
            builder.HasKey(x => x.MessageId);
        }
    }
}