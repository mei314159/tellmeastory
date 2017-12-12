using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TellMe.Web.DAL.Migrations
{
    public partial class UpdateStoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Story");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestDateUtc",
                table: "Story",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDateUtc",
                table: "Story",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviewUrl",
                table: "Story",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "Story",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDateUtc",
                table: "Story");

            migrationBuilder.DropColumn(
                name: "PreviewUrl",
                table: "Story");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "Story");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestDateUtc",
                table: "Story",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Story",
                nullable: true);
        }
    }
}
