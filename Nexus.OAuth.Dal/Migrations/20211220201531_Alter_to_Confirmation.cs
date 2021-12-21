using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.OAuth.Dal.Migrations
{
    public partial class Alter_to_Confirmation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValidationStatus",
                table: "Accounts",
                newName: "ConfirmationStatus");

            migrationBuilder.AddColumn<string>(
                name: "RedirectAuthorize",
                table: "Applications",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RedirectLogin",
                table: "Applications",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RedirectAuthorize",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "RedirectLogin",
                table: "Applications");

            migrationBuilder.RenameColumn(
                name: "ConfirmationStatus",
                table: "Accounts",
                newName: "ValidationStatus");
        }
    }
}
