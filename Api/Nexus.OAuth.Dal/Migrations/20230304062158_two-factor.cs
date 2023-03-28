using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.OAuth.Dal.Migrations
{
    /// <inheritdoc />
    public partial class twofactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TFAEnable",
                table: "Accounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte>(
                name: "TFAType",
                table: "Accounts",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "TFAEnable", "TFAType" },
                values: new object[] { new DateTime(2023, 3, 4, 6, 21, 58, 313, DateTimeKind.Utc).AddTicks(146), true, (byte)0 });

            migrationBuilder.UpdateData(
                table: "Files",
                keyColumn: "Id",
                keyValue: 1,
                column: "Inserted",
                value: new DateTime(2023, 3, 4, 6, 21, 58, 313, DateTimeKind.Utc).AddTicks(269));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TFAEnable",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "TFAType",
                table: "Accounts");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2022, 11, 23, 16, 41, 55, 130, DateTimeKind.Utc).AddTicks(3044));

            migrationBuilder.UpdateData(
                table: "Files",
                keyColumn: "Id",
                keyValue: 1,
                column: "Inserted",
                value: new DateTime(2022, 11, 23, 16, 41, 55, 130, DateTimeKind.Utc).AddTicks(3198));
        }
    }
}
