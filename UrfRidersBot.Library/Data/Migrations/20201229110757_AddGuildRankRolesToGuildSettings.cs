using Microsoft.EntityFrameworkCore.Migrations;

namespace UrfRidersBot.Library.Data.Migrations
{
    public partial class AddGuildRankRolesToGuildSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AdminRoleId",
                table: "GuildSettings",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MemberRoleId",
                table: "GuildSettings",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ModeratorRoleId",
                table: "GuildSettings",
                type: "numeric(20,0)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminRoleId",
                table: "GuildSettings");

            migrationBuilder.DropColumn(
                name: "MemberRoleId",
                table: "GuildSettings");

            migrationBuilder.DropColumn(
                name: "ModeratorRoleId",
                table: "GuildSettings");
        }
    }
}
