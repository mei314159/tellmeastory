using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TellMe.Web.DAL.Migrations
{
    public partial class AddEvents2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoryRequest_Event_EventId",
                table: "StoryRequest");

            migrationBuilder.DropIndex(
                name: "IX_StoryRequest_EventId",
                table: "StoryRequest");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "StoryRequest");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDateUtc",
                table: "EventAttendee",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "StoryRequestId",
                table: "Event",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Event_StoryRequestId",
                table: "Event",
                column: "StoryRequestId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_StoryRequest_StoryRequestId",
                table: "Event",
                column: "StoryRequestId",
                principalTable: "StoryRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_StoryRequest_StoryRequestId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_StoryRequestId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "CreateDateUtc",
                table: "EventAttendee");

            migrationBuilder.DropColumn(
                name: "StoryRequestId",
                table: "Event");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "StoryRequest",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryRequest_EventId",
                table: "StoryRequest",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoryRequest_Event_EventId",
                table: "StoryRequest",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
