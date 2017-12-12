using Microsoft.EntityFrameworkCore.Migrations;

namespace TellMe.Web.DAL.Migrations
{
    public partial class AddCommentsCountColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentsCount",
                table: "Story",
                type: "int",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.Sql("UPDAte Story SET CommentsCount = (SELECT Count(*) FROM Comment WHERE StoryId = Story.Id)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentsCount",
                table: "Story");
        }
    }
}
