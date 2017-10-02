using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TellMe.DAL.Migrations
{
    public partial class UpdateStories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDateUtc",
                table: "Story");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestDateUtc",
                table: "Story",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestDateUtc",
                table: "Story");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDateUtc",
                table: "Story",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
