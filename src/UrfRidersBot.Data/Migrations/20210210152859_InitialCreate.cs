using Microsoft.EntityFrameworkCore.Migrations;

namespace UrfRidersBot.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActiveReactionHandlers",
                columns: table => new
                {
                    MessageId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    TypeName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveReactionHandlers", x => x.MessageId);
                });

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

            migrationBuilder.CreateTable(
                name: "GuildSettings",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    CustomPrefix = table.Column<string>(type: "text", nullable: true),
                    MemberRoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    ModeratorRoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    AdminRoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildSettings", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "ReactionTrackerData",
                columns: table => new
                {
                    MessageId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReactionTrackerData", x => x.MessageId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveReactionHandlers");

            migrationBuilder.DropTable(
                name: "AutoVoiceChannels");

            migrationBuilder.DropTable(
                name: "GuildSettings");

            migrationBuilder.DropTable(
                name: "ReactionTrackerData");
        }
    }
}
