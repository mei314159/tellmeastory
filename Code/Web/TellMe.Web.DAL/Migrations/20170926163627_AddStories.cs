using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TellMe.DAL.Migrations
{
    public partial class AddStories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PushNotificationClient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AppVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OsType = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushNotificationClient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PushNotificationClient_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Story",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Story", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Story_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Story_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PushNotificationClient_UserId",
                table: "PushNotificationClient",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Story_ReceiverId",
                table: "Story",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Story_SenderId",
                table: "Story",
                column: "SenderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PushNotificationClient");

            migrationBuilder.DropTable(
                name: "Story");
        }
    }
}
