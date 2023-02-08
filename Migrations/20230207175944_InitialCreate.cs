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
                    NotBefore = table.Column<long>(type: "INTEGER", nullable: false),
                    Audience = table.Column<string>(type: "TEXT", nullable: true),
                    TokenType = table.Column<string>(type: "TEXT", nullable: true),
                    TokenUse = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Identifier = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthSessions", x => x.Token);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OAuthSessions",
                schema: "BTCPayServer.Plugins.OAuth");
        }
    }
}
