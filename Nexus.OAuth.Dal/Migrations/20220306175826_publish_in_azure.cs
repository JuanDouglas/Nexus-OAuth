using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.OAuth.Dal.Migrations
{
    public partial class publish_in_azure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QrCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Create = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Use = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClientKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Valid = table.Column<bool>(type: "bit", nullable: false),
                    Used = table.Column<bool>(type: "bit", nullable: false),
                    ValidationToken = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IpAdress = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QrCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfirmationStatus = table.Column<short>(type: "smallint", nullable: false),
                    ProfileImageID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(1200)", maxLength: 1200, nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    DirectoryType = table.Column<short>(type: "smallint", nullable: false),
                    Inserted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResourceOwnerId = table.Column<int>(type: "int", nullable: false),
                    Access = table.Column<short>(type: "smallint", nullable: false),
                    Length = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_Accounts_ResourceOwnerId",
                        column: x => x.ResourceOwnerId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FirstSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    IpAdress = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ClientKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Redirect = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FirstSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FirstSteps_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                });

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

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    Secret = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2500)", maxLength: 2500, nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    RedirectLogin = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    RedirectAuthorize = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    LogoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applications_Accounts_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Applications_Files_LogoId",
                        column: x => x.LogoId,
                        principalTable: "Files",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Authorizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresIn = table.Column<double>(type: "float", nullable: true),
                    State = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    ScopesBytes = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authorizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Authorizations_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Authorizations_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Authentications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RefreshToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TokenType = table.Column<short>(type: "smallint", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ExpiresIn = table.Column<double>(type: "float", nullable: true),
                    FirstStepId = table.Column<int>(type: "int", nullable: true),
                    AuthorizationId = table.Column<int>(type: "int", nullable: true),
                    IpAdress = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authentications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Authentications_Authorizations_AuthorizationId",
                        column: x => x.AuthorizationId,
                        principalTable: "Authorizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Authentications_FirstSteps_FirstStepId",
                        column: x => x.FirstStepId,
                        principalTable: "FirstSteps",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Email",
                table: "Accounts",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ProfileImageID",
                table: "Accounts",
                column: "ProfileImageID");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Key",
                table: "Applications",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_LogoId",
                table: "Applications",
                column: "LogoId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_OwnerId",
                table: "Applications",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Authentications_AuthorizationId",
                table: "Authentications",
                column: "AuthorizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Authentications_FirstStepId",
                table: "Authentications",
                column: "FirstStepId");

            migrationBuilder.CreateIndex(
                name: "IX_Authorizations_AccountId",
                table: "Authorizations",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Authorizations_ApplicationId",
                table: "Authorizations",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_ResourceOwnerId",
                table: "Files",
                column: "ResourceOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_FirstSteps_AccountId",
                table: "FirstSteps",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_QrCodeAuthorizations_AccountId",
                table: "QrCodeAuthorizations",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_QrCodeAuthorizations_QrCodeReferenceId",
                table: "QrCodeAuthorizations",
                column: "QrCodeReferenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Files_ProfileImageID",
                table: "Accounts",
                column: "ProfileImageID",
                principalTable: "Files",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Files_ProfileImageID",
                table: "Accounts");

            migrationBuilder.DropTable(
                name: "Authentications");

            migrationBuilder.DropTable(
                name: "QrCodeAuthorizations");

            migrationBuilder.DropTable(
                name: "Authorizations");

            migrationBuilder.DropTable(
                name: "FirstSteps");

            migrationBuilder.DropTable(
                name: "QrCodes");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
