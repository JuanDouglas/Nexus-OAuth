using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.OAuth.Dal.Migrations
{
    public partial class add_seed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Applications_Key",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_OwnerId",
                table: "Applications");

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "ConfirmationStatus", "Created", "Culture", "DateOfBirth", "Email", "Name", "Password", "Phone", "ProfileImageID" },
                values: new object[] { 1, (short)3, new DateTime(2022, 10, 29, 15, 1, 14, 884, DateTimeKind.Utc).AddTicks(9505), "pt-br", new DateTime(2004, 8, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "juandouglas2004@gmail.com", "Juan Douglas Lima da Silva", "$2a$12$GPuArXDC.No0A4gqIADCsOcugLWr8Ij31PubiwS/s.Cj2w/K0KadG", "(61) 99260-6441", null });

            migrationBuilder.InsertData(
                table: "Applications",
                columns: new[] { "Id", "Description", "Key", "LogoId", "MinConfirmationStatus", "Name", "OwnerId", "RedirectAuthorize", "RedirectLogin", "Secret", "Site", "Status" },
                values: new object[] { 1, "The Nexus Energy is a best energy store.", "u5108a260700563169i8686ea59m0850", null, null, "Nexus Energy", 1, "", "", "vazNEwy6EXi2oQ9X68J8Xx3R61KT0LJ6iJ055K29CFEZbCrvyf7a5r7UHs60hRtX49YczrPCXTmo5EnrxLwy3ELMbVA5gHEb", "https://energy.nexus-company.tech/", (short)127 });

            migrationBuilder.InsertData(
                table: "Files",
                columns: new[] { "Id", "Access", "DirectoryType", "FileName", "Inserted", "Length", "ResourceOwnerId", "Type" },
                values: new object[] { 1, (short)1, (short)2, "defaultfile.png", new DateTime(2022, 10, 29, 15, 1, 14, 884, DateTimeKind.Utc).AddTicks(9608), 2333L, 1, (short)2 });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Key",
                table: "Applications",
                column: "Key",
                unique: true)
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_OwnerId",
                table: "Applications",
                column: "OwnerId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Status",
                table: "Applications",
                column: "Status")
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Applications_Key",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_OwnerId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_Status",
                table: "Applications");

            migrationBuilder.DeleteData(
                table: "Applications",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Files",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Key",
                table: "Applications",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_OwnerId",
                table: "Applications",
                column: "OwnerId");
        }
    }
}
