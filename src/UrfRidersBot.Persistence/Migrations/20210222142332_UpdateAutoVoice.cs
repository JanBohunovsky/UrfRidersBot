using Microsoft.EntityFrameworkCore.Migrations;

namespace UrfRidersBot.Persistence.Migrations
{
    public partial class UpdateAutoVoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AutoVoiceChannels",
                table: "AutoVoiceChannels");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AutoVoiceChannels",
                table: "AutoVoiceChannels",
                columns: new[] { "VoiceChannelId", "GuildId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AutoVoiceChannels",
                table: "AutoVoiceChannels");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AutoVoiceChannels",
                table: "AutoVoiceChannels",
                columns: new[] { "GuildId", "VoiceChannelId" });
        }
    }
}
