using Microsoft.EntityFrameworkCore.Migrations;

namespace UrfRidersBot.Persistence.Migrations
{
    public partial class AddAutoVoiceSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoiceBitrate",
                table: "GuildSettings");

            migrationBuilder.CreateTable(
                name: "AutoVoiceSettings",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Bitrate = table.Column<int>(type: "integer", nullable: true),
                    ChannelCreatorId = table.Column<decimal>(type: "numeric(20,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoVoiceSettings", x => x.GuildId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutoVoiceChannels_GuildId",
                table: "AutoVoiceChannels",
                column: "GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_AutoVoiceChannels_AutoVoiceSettings_GuildId",
                table: "AutoVoiceChannels",
                column: "GuildId",
                principalTable: "AutoVoiceSettings",
                principalColumn: "GuildId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutoVoiceChannels_AutoVoiceSettings_GuildId",
                table: "AutoVoiceChannels");

            migrationBuilder.DropTable(
                name: "AutoVoiceSettings");

            migrationBuilder.DropIndex(
                name: "IX_AutoVoiceChannels_GuildId",
                table: "AutoVoiceChannels");

            migrationBuilder.AddColumn<int>(
                name: "VoiceBitrate",
                table: "GuildSettings",
                type: "integer",
                nullable: true);
        }
    }
}
