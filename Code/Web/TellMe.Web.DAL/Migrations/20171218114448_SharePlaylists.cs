using Microsoft.EntityFrameworkCore.Migrations;

namespace TellMe.Web.DAL.Migrations
{
    public partial class SharePlaylists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Playlist_AspNetUsers_UserId",
                table: "Playlist");

            migrationBuilder.DropIndex(
                name: "IX_Playlist_UserId",
                table: "Playlist");
            
            migrationBuilder.CreateTable(
                name: "PlaylistUser",
                columns: table => new
                {
                    PlaylistId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistUser", x => new { x.PlaylistId, x.UserId });
                    table.ForeignKey(
                        name: "FK_PlaylistUser_Playlist_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistUser_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(
                "INSERT INTO PlaylistUser (PlaylistId, UserId, Type) SELECT Id, UserId, 1 FROM Playlist");
            
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Playlist");
            
            migrationBuilder.CreateIndex(
                name: "IX_PlaylistUser_UserId",
                table: "PlaylistUser",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Playlist",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                "UPDATE Playlist SET UserId = (SELECT TOP 1 UserId FROM PlaylistUser WHERE PlaylistId = Id AND Type = 1)");
            
            migrationBuilder.DropTable(
                name: "PlaylistUser");
            
            migrationBuilder.CreateIndex(
                name: "IX_Playlist_UserId",
                table: "Playlist",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Playlist_AspNetUsers_UserId",
                table: "Playlist",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
