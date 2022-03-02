using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.OAuth.Dal.Migrations
{
    public partial class add_application_description : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Applications",
                type: "nvarchar(2500)",
                maxLength: 2500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "QrCodeAuthorizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    QrCodeReferenceId = table.Column<int>(type: "int", nullable: false),
                    AuthorizeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Valid = table.Column<bool>(type: "bit", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QrCodeAuthorizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QrCodeAuthorizations_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QrCodeAuthorizations_QrCodes_QrCodeReferenceId",
                        column: x => x.QrCodeReferenceId,
                        principalTable: "QrCodes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_QrCodeAuthorizations_AccountId",
                table: "QrCodeAuthorizations",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_QrCodeAuthorizations_QrCodeReferenceId",
                table: "QrCodeAuthorizations",
                column: "QrCodeReferenceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QrCodeAuthorizations");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Applications");
        }
    }
}
