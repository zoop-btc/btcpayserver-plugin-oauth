using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BTCPayServer.Plugins.OAuth.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BTCPayServer.Plugins.OAuth;

public class OAuthPluginDbContext : DbContext
{
    private readonly bool _designTime;

    public OAuthPluginDbContext(DbContextOptions<OAuthPluginDbContext> options, bool designTime = false)
        : base(options)
    {
        _designTime = designTime;
    }

    // public DbSet<OauthPluginData> PluginRecords { get; set; }
    public DbSet<OAuthSession> OAuthSessions { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("BTCPayServer.Plugins.OAuth");
        if (Database.IsSqlite() && !_designTime)
            // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
            // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
            // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
            // use the DateTimeOffsetToBinaryConverter
            // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
            // This only supports millisecond precision, but should be sufficient for most use cases.
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                IEnumerable<PropertyInfo> properties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(DateTimeOffset));
                foreach (PropertyInfo property in properties)
                    modelBuilder
                        .Entity(entityType.Name)
                        .Property(property.Name)
                        .HasConversion(new DateTimeOffsetToBinaryConverter());
            }
    }
}
