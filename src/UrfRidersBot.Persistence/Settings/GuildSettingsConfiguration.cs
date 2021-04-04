using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UrfRidersBot.Persistence.Settings
{
    public class GuildSettingsConfiguration : IEntityTypeConfiguration<Core.Settings.GuildSettings>
    {
        public void Configure(EntityTypeBuilder<Core.Settings.GuildSettings> builder)
        {
            builder.HasKey(x => x.GuildId);
        }
    }
}