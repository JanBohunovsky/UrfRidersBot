using Microsoft.EntityFrameworkCore.Migrations;

namespace UrfRidersBot.Persistence.Migrations
{
    public partial class AddColorRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ColorRoles",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    RoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorRoles", x => new { x.GuildId, x.RoleId, x.UserId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColorRoles_GuildId_RoleId",
                table: "ColorRoles",
                columns: new[] { "GuildId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ColorRoles_GuildId_UserId",
                table: "ColorRoles",
                columns: new[] { "GuildId", "UserId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColorRoles");
        }
    }
}
