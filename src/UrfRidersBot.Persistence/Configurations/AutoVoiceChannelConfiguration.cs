using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrfRidersBot.Core.Entities;

namespace UrfRidersBot.Persistence.Configurations
{
    public class AutoVoiceChannelConfiguration : IEntityTypeConfiguration<AutoVoiceChannel>
    {
        public void Configure(EntityTypeBuilder<AutoVoiceChannel> builder)
        {
            builder.HasKey(x => new { x.GuildId, x.VoiceChannelId });
        }
    }
}