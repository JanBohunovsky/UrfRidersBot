﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using UrfRidersBot.Persistence.Common;

namespace UrfRidersBot.Persistence.Migrations
{
    [DbContext(typeof(UrfRidersDbContext))]
    partial class UrfRidersDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("UrfRidersBot.Core.AutoVoice.AutoVoiceChannel", b =>
                {
                    b.Property<decimal>("VoiceChannelId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("VoiceChannelId", "GuildId");

                    b.HasIndex("GuildId");

                    b.ToTable("AutoVoiceChannels");
                });

            modelBuilder.Entity("UrfRidersBot.Core.AutoVoice.AutoVoiceSettings", b =>
                {
                    b.Property<decimal>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<int?>("Bitrate")
                        .HasColumnType("integer");

                    b.Property<decimal?>("ChannelCreatorId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("GuildId");

                    b.ToTable("AutoVoiceSettings");
                });

            modelBuilder.Entity("UrfRidersBot.Core.Settings.GuildSettings", b =>
                {
                    b.Property<decimal>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal?>("AdminRoleId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal?>("MemberRoleId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal?>("ModeratorRoleId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("GuildId");

                    b.ToTable("GuildSettings");
                });

            modelBuilder.Entity("UrfRidersBot.Persistence.ColorRole.ColorRoleDTO", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("RoleId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("GuildId", "RoleId", "UserId");

                    b.HasIndex("GuildId", "RoleId")
                        .IsUnique();

                    b.HasIndex("GuildId", "UserId")
                        .IsUnique();

                    b.ToTable("ColorRoles");
                });

            modelBuilder.Entity("UrfRidersBot.Persistence.ReactionRoles.ReactionRoleDTO", b =>
                {
                    b.Property<decimal>("MessageId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Emoji")
                        .HasColumnType("text");

                    b.Property<decimal>("RoleId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("MessageId", "Emoji", "RoleId");

                    b.HasIndex("MessageId", "Emoji")
                        .IsUnique();

                    b.HasIndex("MessageId", "RoleId")
                        .IsUnique();

                    b.ToTable("ReactionRoles");
                });

            modelBuilder.Entity("UrfRidersBot.Core.AutoVoice.AutoVoiceChannel", b =>
                {
                    b.HasOne("UrfRidersBot.Core.AutoVoice.AutoVoiceSettings", null)
                        .WithMany("VoiceChannels")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("UrfRidersBot.Core.AutoVoice.AutoVoiceSettings", b =>
                {
                    b.Navigation("VoiceChannels");
                });
#pragma warning restore 612, 618
        }
    }
}
