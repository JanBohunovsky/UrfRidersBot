using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrfRidersBot.Core.Entities;

namespace UrfRidersBot.Persistence.Configurations
{
    public class GuildSettingsConfiguration : IEntityTypeConfiguration<GuildSettings>
    {
        public void Configure(EntityTypeBuilder<GuildSettings> builder)
        {
            builder.HasKey(x => x.GuildId);
        }
    }
}