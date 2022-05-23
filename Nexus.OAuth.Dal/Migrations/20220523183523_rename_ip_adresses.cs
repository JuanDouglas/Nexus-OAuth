using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.OAuth.Dal.Migrations
{
    public partial class rename_ip_adresses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Adress",
                table: "QrCodes",
                newName: "IpAdress");

            migrationBuilder.RenameColumn(
                name: "Adress",
                table: "FirstSteps",
                newName: "IpAdress");

            migrationBuilder.RenameColumn(
                name: "Adress",
                table: "Authentications",
                newName: "IpAdress");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IpAdress",
                table: "QrCodes",
                newName: "Adress");

            migrationBuilder.RenameColumn(
                name: "IpAdress",
                table: "FirstSteps",
                newName: "Adress");

            migrationBuilder.RenameColumn(
                name: "IpAdress",
                table: "Authentications",
                newName: "Adress");
        }
    }
}
