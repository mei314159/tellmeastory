using Microsoft.EntityFrameworkCore.Migrations;

namespace TellMe.Web.DAL.Migrations
{
    public partial class ReplyToComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReplyToCommentId",
                table: "Comment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ReplyToCommentId",
                table: "Comment",
                column: "ReplyToCommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Comment_ReplyToCommentId",
                table: "Comment",
                column: "ReplyToCommentId",
                principalTable: "Comment",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Comment_ReplyToCommentId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_ReplyToCommentId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "ReplyToCommentId",
                table: "Comment");
        }
    }
}
