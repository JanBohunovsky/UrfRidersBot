using Microsoft.EntityFrameworkCore.Migrations;

namespace UrfRidersBot.Persistence.Migrations
{
    public partial class AddReactionRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReactionRoles",
                columns: table => new
                {
                    MessageId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Emoji = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReactionRoles", x => new { x.MessageId, x.Emoji, x.RoleId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReactionRoles_MessageId_Emoji",
                table: "ReactionRoles",
                columns: new[] { "MessageId", "Emoji" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReactionRoles_MessageId_RoleId",
                table: "ReactionRoles",
                columns: new[] { "MessageId", "RoleId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReactionRoles");
        }
    }
}
