using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BTCPayServer.Plugins.OAuth.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "BTCPayServer.Plugins.OAuth");

            migrationBuilder.CreateTable(
                name: "ExtraData",
                schema: "BTCPayServer.Plugins.OAuth",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Identifier = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OAuthSessions",
                schema: "BTCPayServer.Plugins.OAuth",
                columns: table => new
                {
                    Token = table.Column<string>(type: "TEXT", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    Scope = table.Column<string>(type: "TEXT", nullable: true),
                    ClientId = table.Column<string>(type: "TEXT", nullable: true),
                    Issuer = table.Column<string>(type: "TEXT", nullable: true),
                    Subject = table.Column<string>(type: "TEXT", nullable: true),
                    ExpiresAt = table.Column<long>(type: "INTEGER", nullable: false),
                    IssuedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    AuthenticatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    NotBefore = table.Column<long>(type: "INTEGER", nullable: false),
                    TokenType = table.Column<string>(type: "TEXT", nullable: true),
                    TokenUse = table.Column<string>(type: "TEXT", nullable: true),
                    ExtraId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthSessions", x => x.Token);
                    table.ForeignKey(
                        name: "FK_OAuthSessions_ExtraData_ExtraId",
                        column: x => x.ExtraId,
                        principalSchema: "BTCPayServer.Plugins.OAuth",
                        principalTable: "ExtraData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OAuthSessions_ExtraId",
                schema: "BTCPayServer.Plugins.OAuth",
                table: "OAuthSessions",
                column: "ExtraId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OAuthSessions",
                schema: "BTCPayServer.Plugins.OAuth");

            migrationBuilder.DropTable(
                name: "ExtraData",
                schema: "BTCPayServer.Plugins.OAuth");
        }
    }
}
