using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.OAuth.Dal.Migrations
{
    public partial class qr_code_reference_authentication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QrCodeAuthorizationId",
                table: "Authentications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Authentications_QrCodeAuthorizationId",
                table: "Authentications",
                column: "QrCodeAuthorizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Authentications_QrCodeAuthorizations_QrCodeAuthorizationId",
                table: "Authentications",
                column: "QrCodeAuthorizationId",
                principalTable: "QrCodeAuthorizations",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authentications_QrCodeAuthorizations_QrCodeAuthorizationId",
                table: "Authentications");

            migrationBuilder.DropIndex(
                name: "IX_Authentications_QrCodeAuthorizationId",
                table: "Authentications");

            migrationBuilder.DropColumn(
                name: "QrCodeAuthorizationId",
                table: "Authentications");
        }
    }
}
