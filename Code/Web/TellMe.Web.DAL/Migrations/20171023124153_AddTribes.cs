using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TellMe.DAL.Migrations
{
    public partial class AddTribes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Story WHERE Status != 2");
            migrationBuilder.DropForeignKey(
                name: "FK_Story_AspNetUsers_ReceiverId",
                table: "Story");

            migrationBuilder.DropIndex(
                name: "IX_Story_ReceiverId",
                table: "Story");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "Story");

            migrationBuilder.DropColumn(
                name: "RequestDateUtc",
                table: "Story");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Story");

            migrationBuilder.DropColumn(
                name: "UpdateDateUtc",
                table: "Story");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDateUtc",
                table: "Story",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "Story",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tribe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tribe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoryReceiver",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StoryId = table.Column<int>(type: "int", nullable: false),
                    TribeId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryReceiver", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryReceiver_Story_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Story",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryReceiver_Tribe_TribeId",
                        column: x => x.TribeId,
                        principalTable: "Tribe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoryReceiver_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StoryRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TribeId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryRequest_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoryRequest_Tribe_TribeId",
                        column: x => x.TribeId,
                        principalTable: "Tribe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoryRequest_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TribeMember",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TribeId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TribeMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TribeMember_Tribe_TribeId",
                        column: x => x.TribeId,
                        principalTable: "Tribe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TribeMember_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StoryRequestStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TribeId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryRequestStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryRequestStatus_StoryRequest_RequestId",
                        column: x => x.RequestId,
                        principalTable: "StoryRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryRequestStatus_Tribe_TribeId",
                        column: x => x.TribeId,
                        principalTable: "Tribe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoryRequestStatus_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Story_RequestId",
                table: "Story",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryReceiver_StoryId",
                table: "StoryReceiver",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryReceiver_TribeId",
                table: "StoryReceiver",
                column: "TribeId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryReceiver_UserId",
                table: "StoryReceiver",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryRequest_SenderId",
                table: "StoryRequest",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryRequest_TribeId",
                table: "StoryRequest",
                column: "TribeId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryRequest_UserId",
                table: "StoryRequest",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryRequestStatus_RequestId",
                table: "StoryRequestStatus",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryRequestStatus_TribeId",
                table: "StoryRequestStatus",
                column: "TribeId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryRequestStatus_UserId",
                table: "StoryRequestStatus",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TribeMember_TribeId",
                table: "TribeMember",
                column: "TribeId");

            migrationBuilder.CreateIndex(
                name: "IX_TribeMember_UserId",
                table: "TribeMember",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Story_StoryRequest_RequestId",
                table: "Story",
                column: "RequestId",
                principalTable: "StoryRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Story_StoryRequest_RequestId",
                table: "Story");

            migrationBuilder.DropTable(
                name: "StoryReceiver");

            migrationBuilder.DropTable(
                name: "StoryRequestStatus");

            migrationBuilder.DropTable(
                name: "TribeMember");

            migrationBuilder.DropTable(
                name: "StoryRequest");

            migrationBuilder.DropTable(
                name: "Tribe");

            migrationBuilder.DropIndex(
                name: "IX_Story_RequestId",
                table: "Story");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Story");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDateUtc",
                table: "Story",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverId",
                table: "Story",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestDateUtc",
                table: "Story",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Story",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDateUtc",
                table: "Story",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Story_ReceiverId",
                table: "Story",
                column: "ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Story_AspNetUsers_ReceiverId",
                table: "Story",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
