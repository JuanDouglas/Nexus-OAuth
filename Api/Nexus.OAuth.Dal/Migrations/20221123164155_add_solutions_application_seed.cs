using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.OAuth.Dal.Migrations
{
    public partial class add_solutions_application_seed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2022, 11, 23, 16, 41, 55, 130, DateTimeKind.Utc).AddTicks(3044));

            migrationBuilder.UpdateData(
                table: "Applications",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "Site" },
                values: new object[] { "Nexus Solutions", "https://solutions.nexus-company.tech/" });

            migrationBuilder.UpdateData(
                table: "Files",
                keyColumn: "Id",
                keyValue: 1,
                column: "Inserted",
                value: new DateTime(2022, 11, 23, 16, 41, 55, 130, DateTimeKind.Utc).AddTicks(3198));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2022, 11, 23, 16, 39, 50, 975, DateTimeKind.Utc).AddTicks(3679));

            migrationBuilder.UpdateData(
                table: "Applications",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "Site" },
                values: new object[] { "Nexus Solution", "https://solution.nexus-company.tech/" });

            migrationBuilder.UpdateData(
                table: "Files",
                keyColumn: "Id",
                keyValue: 1,
                column: "Inserted",
                value: new DateTime(2022, 11, 23, 16, 39, 50, 975, DateTimeKind.Utc).AddTicks(3960));
        }
    }
}
