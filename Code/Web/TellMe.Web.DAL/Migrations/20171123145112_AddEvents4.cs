using Microsoft.EntityFrameworkCore.Migrations;

namespace TellMe.Web.DAL.Migrations
{
    public partial class AddEvents4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendee_Tribe_TribeId",
                table: "EventAttendee");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "EventAttendee",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendee_Tribe_TribeId",
                table: "EventAttendee",
                column: "TribeId",
                principalTable: "Tribe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendee_Tribe_TribeId",
                table: "EventAttendee");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "EventAttendee",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendee_Tribe_TribeId",
                table: "EventAttendee",
                column: "TribeId",
                principalTable: "Tribe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
