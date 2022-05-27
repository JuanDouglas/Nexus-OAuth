using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.OAuth.Dal.Migrations
{
    public partial class add_culture_in_account : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "IpAdress",
                table: "FirstSteps",
                type: "varbinary(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(6)",
                oldMaxLength: 6);

            migrationBuilder.AlterColumn<byte[]>(
                name: "IpAdress",
                table: "Authentications",
                type: "varbinary(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(6)",
                oldMaxLength: 6);

            migrationBuilder.AddColumn<string>(
                name: "Culture",
                table: "Accounts",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "pt-BR");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Culture",
                table: "Accounts");

            migrationBuilder.AlterColumn<byte[]>(
                name: "IpAdress",
                table: "FirstSteps",
                type: "varbinary(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)",
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<byte[]>(
                name: "IpAdress",
                table: "Authentications",
                type: "varbinary(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)",
                oldMaxLength: 16);
        }
    }
}
