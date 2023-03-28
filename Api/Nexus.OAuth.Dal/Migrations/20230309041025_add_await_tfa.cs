using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.OAuth.Dal.Migrations
{
    /// <inheritdoc />
    public partial class add_await_tfa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AwaitTFA",
                table: "Authentications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2023, 3, 9, 4, 10, 24, 822, DateTimeKind.Utc).AddTicks(4798));

            migrationBuilder.UpdateData(
                table: "Files",
                keyColumn: "Id",
                keyValue: 1,
                column: "Inserted",
                value: new DateTime(2023, 3, 9, 4, 10, 24, 822, DateTimeKind.Utc).AddTicks(4889));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwaitTFA",
                table: "Authentications");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2023, 3, 4, 6, 21, 58, 313, DateTimeKind.Utc).AddTicks(146));

            migrationBuilder.UpdateData(
                table: "Files",
                keyColumn: "Id",
                keyValue: 1,
                column: "Inserted",
                value: new DateTime(2023, 3, 4, 6, 21, 58, 313, DateTimeKind.Utc).AddTicks(269));
        }
    }
}
