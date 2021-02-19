using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrfRidersBot.Core.Entities;
using UrfRidersBot.Persistence.DTOs;

namespace UrfRidersBot.Persistence.Configurations
{
    public class AutoVoiceChannelConfiguration : IEntityTypeConfiguration<AutoVoiceChannelDTO>
    {
        public void Configure(EntityTypeBuilder<AutoVoiceChannelDTO> builder)
        {
            builder.HasKey(x => new { x.GuildId, x.VoiceChannelId });
        }
    }
}