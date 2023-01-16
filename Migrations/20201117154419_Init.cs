using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BTCPayServer.Plugins.OAuth.Migrations
{
    [DbContext(typeof(PluginDbContext))]
    [Migration("20201117154419_Init")]
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "BTCPayServer.Plugins.OAuth");

            migrationBuilder.CreateTable(
                name: "PluginRecords",
                schema: "BTCPayServer.Plugins.OAuth",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PluginRecords", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PluginRecords",
                schema: "BTCPayServer.Plugins.OAuth");
        }
    }
}
