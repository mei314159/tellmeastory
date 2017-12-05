using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TellMe.DAL.Migrations
{
    public partial class ChangeAuthFlow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contact");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PhoneCountryCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PhoneNumberDigits",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Friendship",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FriendId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friendship", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Friendship_AspNetUsers_FriendId",
                        column: x => x.FriendId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Friendship_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_FriendId",
                table: "Friendship",
                column: "FriendId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_UserId",
                table: "Friendship",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friendship");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "AspNetUsers",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PhoneCountryCode",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<long>(
                name: "PhoneNumberDigits",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberDigits = table.Column<long>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contact_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contact_UserId",
                table: "Contact",
                column: "UserId");
        }
    }
}
