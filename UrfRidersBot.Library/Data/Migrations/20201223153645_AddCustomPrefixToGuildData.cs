using Microsoft.EntityFrameworkCore.Migrations;

namespace UrfRidersBot.Library.Data.Migrations
{
    public partial class AddCustomPrefixToGuildData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RandomValue",
                table: "GuildData",
                newName: "CustomPrefix");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomPrefix",
                table: "GuildData",
                newName: "RandomValue");
        }
    }
}
