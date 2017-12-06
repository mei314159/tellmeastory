using Microsoft.EntityFrameworkCore.Migrations;

namespace TellMe.Web.DAL.Migrations
{
    public partial class AddTribeCreator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Tribe",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tribe_CreatorId",
                table: "Tribe",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tribe_AspNetUsers_CreatorId",
                table: "Tribe",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tribe_AspNetUsers_CreatorId",
                table: "Tribe");

            migrationBuilder.DropIndex(
                name: "IX_Tribe_CreatorId",
                table: "Tribe");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Tribe");
        }
    }
}
