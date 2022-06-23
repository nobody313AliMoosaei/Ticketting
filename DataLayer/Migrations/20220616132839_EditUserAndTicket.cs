using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class EditUserAndTicket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CounterRequest",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CounterReport",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CounterRequest", "Email", "FullName", "HashPassword", "IsActive", "IsAdmin", "IsConfirmEmail", "IsRemove", "UserName" },
                values: new object[] { 10, 0, "ali.moosaei.big@gmail.com", "AliMoosaei", "xXUCWL1n3GsjBs+TV4AS6kFm8xnugr8bGPjINMcu4mw=", true, true, true, false, "AliMoosaei" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "CounterRequest",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CounterReport",
                table: "Tickets");
        }
    }
}
