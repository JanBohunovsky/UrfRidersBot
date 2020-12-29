﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using UrfRidersBot.Library;

namespace UrfRidersBot.Library.Data.Migrations
{
    [DbContext(typeof(UrfRidersDbContext))]
    [Migration("20201228093308_RenameGuildDataToGuildSettings")]
    partial class RenameGuildDataToGuildSettings
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("UrfRidersBot.Library.GuildSettings", b =>
                {
                    b.Property<decimal>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("CustomPrefix")
                        .HasColumnType("text");

                    b.HasKey("GuildId");

                    b.ToTable("GuildSettings");
                });
#pragma warning restore 612, 618
        }
    }
}