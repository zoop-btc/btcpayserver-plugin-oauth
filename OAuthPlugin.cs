using BTCPayServer.Abstractions.Contracts;
using BTCPayServer.Abstractions.Extensions;
using BTCPayServer.Abstractions.Models;
using BTCPayServer.Abstractions.Services;
using BTCPayServer.Plugins.OAuth.Auth;
using BTCPayServer.Plugins.OAuth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BTCPayServer.Plugins.OAuth;

public class OAuthPlugin : BaseBTCPayServerPlugin
{
    public override string Identifier { get; } = "BTCPayServer.Plugins.OAuth";
    public override string Name { get; } = "OAuth Plugin";
    public override string Description { get; } = "This plugin makes BTCPayServer a resource provider by verifying OAuth2 access tokens for the GreenField api.";

    public override void Execute(IServiceCollection services)
    {
        services.AddStartupTask<OAuthInitConfig>();
        services.AddSingleton<IUIExtension>(new UIExtension("OAuthPluginHeaderNav", "header-nav"));
        services.AddHostedService<PluginMigrationRunner>();
        services.AddSingleton<OAuthService>();
        services.AddSingleton<OAuthDbContextFactory>();
        services.AddDbContext<OAuthPluginDbContext>((provider, o) =>
        {
            OAuthDbContextFactory factory = provider.GetRequiredService<OAuthDbContextFactory>();
            factory.ConfigureBuilder(o);
        });
        services.AddHostedService<OAuthTokenCleanup>();
        services.AddAuthentication()
                .AddScheme<OAuthAuthenticationOptions, OAuthAPIAuthenticationHandler>("OAuth.API", options => { });
        services.Configure<MvcOptions>(options =>
        {
            options.Conventions.Add(new OAuthControllerConvention());
            options.Conventions.Add(new OAuthActionConvention());
        });
    }
}
