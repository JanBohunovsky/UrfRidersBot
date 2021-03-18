using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrfRidersBot.Core.Entities;

namespace UrfRidersBot.Persistence.Configurations
{
    public class AutoVoiceSettingsConfiguration : IEntityTypeConfiguration<AutoVoiceSettings>
    {
        public void Configure(EntityTypeBuilder<AutoVoiceSettings> builder)
        {
            builder.HasKey(s => s.GuildId);

            builder
                .Navigation(x => x.VoiceChannels)
                .HasField("_voiceChannels");
        }
    }
}