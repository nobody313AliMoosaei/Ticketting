using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class EditToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenBase",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "TokenHash",
                table: "Tokens");

            migrationBuilder.AddColumn<bool>(
                name: "IsExpire",
                table: "Tokens",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeInsert",
                table: "Tokens",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TokenValue",
                table: "Tokens",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExpire",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "TimeInsert",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "TokenValue",
                table: "Tokens");

            migrationBuilder.AddColumn<string>(
                name: "TokenBase",
                table: "Tokens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenHash",
                table: "Tokens",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
