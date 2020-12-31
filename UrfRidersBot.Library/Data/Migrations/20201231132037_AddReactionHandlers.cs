using Microsoft.EntityFrameworkCore.Migrations;

namespace UrfRidersBot.Library.Data.Migrations
{
    public partial class AddReactionHandlers : Migration
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
                name: "ReactionTrackerData");
        }
    }
}
