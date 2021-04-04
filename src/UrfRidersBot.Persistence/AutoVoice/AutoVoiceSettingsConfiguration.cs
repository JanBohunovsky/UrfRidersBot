using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrfRidersBot.Core.AutoVoice;

namespace UrfRidersBot.Persistence.AutoVoice
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