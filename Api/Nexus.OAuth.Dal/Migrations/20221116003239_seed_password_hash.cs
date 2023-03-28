using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.OAuth.Dal.Migrations
{
    public partial class seed_password_hash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2022, 11, 16, 0, 32, 38, 629, DateTimeKind.Utc).AddTicks(3591), "$2a$10$dRVgoKzNY1ir9B8CGhUkPO4WYsZzXpcOyZriz6th1VzbuCK.DDMIS" });

            migrationBuilder.UpdateData(
                table: "Applications",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "RedirectAuthorize", "RedirectLogin" },
                values: new object[] { "https://energy.nexus-company.tech/oauth/authorize", "https://energy.nexus-company.tech/oauth/login" });

            migrationBuilder.UpdateData(
                table: "Files",
                keyColumn: "Id",
                keyValue: 1,
                column: "Inserted",
                value: new DateTime(2022, 11, 16, 0, 32, 38, 629, DateTimeKind.Utc).AddTicks(3802));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2022, 10, 29, 15, 1, 14, 884, DateTimeKind.Utc).AddTicks(9505), "$2a$12$GPuArXDC.No0A4gqIADCsOcugLWr8Ij31PubiwS/s.Cj2w/K0KadG" });

            migrationBuilder.UpdateData(
                table: "Applications",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "RedirectAuthorize", "RedirectLogin" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Files",
                keyColumn: "Id",
                keyValue: 1,
                column: "Inserted",
                value: new DateTime(2022, 10, 29, 15, 1, 14, 884, DateTimeKind.Utc).AddTicks(9608));
        }
    }
}
