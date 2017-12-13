using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TellMe.Web.DAL.Migrations
{
    public partial class AddOrderToPlaylist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateUtc",
                table: "PlaylistStory");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "PlaylistStory",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "PlaylistStory");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUtc",
                table: "PlaylistStory",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
