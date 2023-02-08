#nullable enable
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BTCPayServer.Abstractions.Contracts;
using BTCPayServer.Plugins.OAuth;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class OAuthTokenCleanup : IHostedService, IDisposable
{
    private readonly ILogger<OAuthTokenCleanup> _logger;
    private Timer? _timer = null;

    private readonly OAuthDbContextFactory _OAuthDbContextFactory;


    public OAuthTokenCleanup(ILogger<OAuthTokenCleanup> logger, OAuthDbContextFactory OAuthDbContextFactory)
    {
        _logger = logger;
        _OAuthDbContextFactory = OAuthDbContextFactory;

    }

    public Task StartAsync(CancellationToken stoppingToken)
    {

        _timer = new Timer(DeleteExpiredTokens, null, TimeSpan.Zero,
            TimeSpan.FromDays(1));

        return Task.CompletedTask;
    }

    private async void DeleteExpiredTokens(object? state)
    {

        await using OAuthPluginDbContext context = _OAuthDbContextFactory.CreateContext();

        var currenttime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var todelete = context.OAuthSessions.Where(s => s.ExpiresAt < currenttime).ToList();
        if (todelete.Any())
        {
            _logger.LogInformation($"Removing {todelete.Count} expired tokens from database");
            context.OAuthSessions.RemoveRange(todelete);
            context.SaveChanges();
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OAuth Token Cleanup is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}