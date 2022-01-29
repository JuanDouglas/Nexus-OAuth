using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.OAuth.Dal.Migrations
{
    public partial class Add_resource_owner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResourceOwnerId",
                table: "Files",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Files_ResourceOwnerId",
                table: "Files",
                column: "ResourceOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Accounts_ResourceOwnerId",
                table: "Files",
                column: "ResourceOwnerId",
                principalTable: "Accounts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Accounts_ResourceOwnerId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_ResourceOwnerId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ResourceOwnerId",
                table: "Files");
        }
    }
}
