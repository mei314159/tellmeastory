using Microsoft.EntityFrameworkCore.Migrations;

namespace TellMe.DAL.Migrations
{
    public partial class UpdateRefreshTokensTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStop",
                table: "RefreshTokens");

            migrationBuilder.AddColumn<bool>(
                name: "Expired",
                table: "RefreshTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Expired",
                table: "RefreshTokens");

            migrationBuilder.AddColumn<int>(
                name: "IsStop",
                table: "RefreshTokens",
                nullable: false,
                defaultValue: 0);
        }
    }
}
