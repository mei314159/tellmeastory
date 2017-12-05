using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TellMe.DAL.Migrations
{
    public partial class AddComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreateDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StoryId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comment_Story_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Story",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_AuthorId",
                table: "Comment",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_StoryId",
                table: "Comment",
                column: "StoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");
        }
    }
}
