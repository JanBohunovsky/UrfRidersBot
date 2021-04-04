using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrfRidersBot.Core.AutoVoice;

namespace UrfRidersBot.Persistence.AutoVoice
{
    public class AutoVoiceChannelConfiguration : IEntityTypeConfiguration<AutoVoiceChannel>
    {
        public void Configure(EntityTypeBuilder<AutoVoiceChannel> builder)
        {
            builder.HasKey(x => new { x.VoiceChannelId, x.GuildId });

            builder
                .HasOne<AutoVoiceSettings>()
                .WithMany(s => s.VoiceChannels)
                .HasForeignKey(c => c.GuildId);

            builder.ToTable("AutoVoiceChannels");
        }
    }
}