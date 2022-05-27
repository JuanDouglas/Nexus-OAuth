using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.OAuth.Dal.Migrations
{
    public partial class update_ip_adress_types : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAdress",
                table: "QrCodes");

            migrationBuilder.DropColumn(
                name: "IpAdress",
                table: "FirstSteps");

            migrationBuilder.DropColumn(
                name: "IpAdress",
                table: "Authentications");

            migrationBuilder.AddColumn<byte[]>(
                name: "Adress",
                table: "QrCodes",
                type: "varbinary(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "Adress",
                table: "FirstSteps",
                type: "varbinary(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "Adress",
                table: "Authentications",
                type: "varbinary(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adress",
                table: "QrCodes");

            migrationBuilder.DropColumn(
                name: "Adress",
                table: "FirstSteps");

            migrationBuilder.DropColumn(
                name: "Adress",
                table: "Authentications");

            migrationBuilder.AddColumn<string>(
                name: "IpAdress",
                table: "QrCodes",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpAdress",
                table: "FirstSteps",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpAdress",
                table: "Authentications",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");
        }
    }
}
