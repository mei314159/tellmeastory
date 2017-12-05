using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TellMe.DAL.Migrations
{
    public partial class AddPlaylists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Playlist",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateDateUtc = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Playlist_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistStory",
                columns: table => new
                {
                    PlaylistId = table.Column<int>(nullable: false),
                    StoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistStory", x => new { x.PlaylistId, x.StoryId });
                    table.ForeignKey(
                        name: "FK_PlaylistStory_Playlist_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistStory_Story_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Story",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Playlist_UserId",
                table: "Playlist",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistStory_StoryId",
                table: "PlaylistStory",
                column: "StoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaylistStory");

            migrationBuilder.DropTable(
                name: "Playlist");
        }
    }
}
