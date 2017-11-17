using Microsoft.EntityFrameworkCore.Migrations;

namespace TellMe.DAL.Migrations
{
    public partial class AddRepliesCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Comment_ReplyToCommentId",
                table: "Comment");

            migrationBuilder.AddColumn<int>(
                name: "RepliesCount",
                table: "Comment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Comment_ReplyToCommentId",
                table: "Comment",
                column: "ReplyToCommentId",
                principalTable: "Comment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Comment_ReplyToCommentId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "RepliesCount",
                table: "Comment");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Comment_ReplyToCommentId",
                table: "Comment",
                column: "ReplyToCommentId",
                principalTable: "Comment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
