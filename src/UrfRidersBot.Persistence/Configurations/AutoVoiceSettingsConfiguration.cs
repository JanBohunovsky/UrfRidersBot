﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrfRidersBot.Core.Entities;

namespace UrfRidersBot.Persistence.Configurations
{
    public class AutoVoiceSettingsConfiguration : IEntityTypeConfiguration<AutoVoiceSettings>
    {
        public void Configure(EntityTypeBuilder<AutoVoiceSettings> builder)
        {
            
        }
    }
}