using System.Threading;
using System.Threading.Tasks;
using BTCPayServer.Abstractions.Contracts;
using BTCPayServer.Plugins.OAuth.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace BTCPayServer.Plugins.OAuth;

public class PluginMigrationRunner : IHostedService
{
    private readonly OAuthDbContextFactory _PluginDbContextFactory;
    private readonly OAuthService _PluginService;
    private readonly ISettingsRepository _settingsRepository;

    public PluginMigrationRunner(OAuthDbContextFactory PluginDbContextFactory, ISettingsRepository settingsRepository,
        OAuthService PluginService)
    {
        _PluginDbContextFactory = PluginDbContextFactory;
        _settingsRepository = settingsRepository;
        _PluginService = PluginService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        PluginDataMigrationHistory settings = await _settingsRepository.GetSettingAsync<PluginDataMigrationHistory>() ??
                                              new PluginDataMigrationHistory();
        await using OAuthPluginDbContext ctx = _PluginDbContextFactory.CreateContext();
        await ctx.Database.MigrateAsync(cancellationToken);
        
        // settings migrations
        if (!settings.UpdatedSomething)
        {
            settings.UpdatedSomething = true;
            await _settingsRepository.UpdateSetting(settings);
        }
        
        // test record
       // await _PluginService.AddTestDataRecord();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public class PluginDataMigrationHistory
    {
        public bool UpdatedSomething { get; set; }
    }
}

