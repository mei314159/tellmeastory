using Microsoft.EntityFrameworkCore.Migrations;

namespace TellMe.DAL.Migrations
{
    public partial class AddHandledFlagToNotificationsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Handled",
                table: "Notification",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Handled",
                table: "Notification");
        }
    }
}
