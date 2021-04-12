using Microsoft.EntityFrameworkCore.Migrations;

namespace UrfRidersBot.Persistence.Migrations
{
    public partial class RemoveCustomPrefixFromGuildSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomPrefix",
                table: "GuildSettings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomPrefix",
                table: "GuildSettings",
                type: "text",
                nullable: true);
        }
    }
}
