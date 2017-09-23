using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TellMe.DAL.Migrations
{
    public partial class ChangeTypeOfPhoneNumberDigits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "PhoneNumberDigits",
                table: "Contact",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "PhoneNumberDigits",
                table: "AspNetUsers",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PhoneNumberDigits",
                table: "Contact",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "PhoneNumberDigits",
                table: "AspNetUsers",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
