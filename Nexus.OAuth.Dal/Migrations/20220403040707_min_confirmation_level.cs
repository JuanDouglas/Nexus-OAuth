using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.OAuth.Dal.Migrations
{
    public partial class min_confirmation_level : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "MinConfirmationStatus",
                table: "Applications",
                type: "smallint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinConfirmationStatus",
                table: "Applications");
        }
    }
}
