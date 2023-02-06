using BTCPayServer.Abstractions.Contracts;
using BTCPayServer.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace BTCPayServer.Plugins.OAuth;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OAuthPluginDbContext>
{
    public OAuthPluginDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<OAuthPluginDbContext> builder = new DbContextOptionsBuilder<OAuthPluginDbContext>();

        builder.UseSqlite("Data Source=temp.db");

        return new OAuthPluginDbContext(builder.Options, true);
    }
}

public class OAuthDbContextFactory : BaseDbContextFactory<OAuthPluginDbContext>
{
    public OAuthDbContextFactory(IOptions<DatabaseOptions> options) : base(options, "BTCPayServer.Plugins.OAuth")
    {
    }

    public override OAuthPluginDbContext CreateContext()
    {
        DbContextOptionsBuilder<OAuthPluginDbContext> builder = new DbContextOptionsBuilder<OAuthPluginDbContext>();
        ConfigureBuilder(builder);
        return new OAuthPluginDbContext(builder.Options);
    }
}
