using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TellMe.DAL.Migrations
{
    public partial class AddEvents1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TribeId",
                table: "EventAttendee",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShareStories",
                table: "Event",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendee_TribeId",
                table: "EventAttendee",
                column: "TribeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendee_Tribe_TribeId",
                table: "EventAttendee",
                column: "TribeId",
                principalTable: "Tribe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendee_Tribe_TribeId",
                table: "EventAttendee");

            migrationBuilder.DropIndex(
                name: "IX_EventAttendee_TribeId",
                table: "EventAttendee");

            migrationBuilder.DropColumn(
                name: "TribeId",
                table: "EventAttendee");

            migrationBuilder.DropColumn(
                name: "ShareStories",
                table: "Event");
        }
    }
}
