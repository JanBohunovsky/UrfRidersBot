using Microsoft.EntityFrameworkCore.Migrations;

namespace UrfRidersBot.Database.Migrations
{
    public partial class AddAutoVoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AutoVoiceChannels",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    VoiceChannelId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoVoiceChannels", x => new { x.GuildId, x.VoiceChannelId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutoVoiceChannels");
        }
    }
}
