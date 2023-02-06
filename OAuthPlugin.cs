using BTCPayServer.Abstractions.Contracts;
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
    public override string Name { get; } = "Plugin Template";
    public override string Description { get; } = "This is the plugin description";

    public override void Execute(IServiceCollection services)
    {
        services.AddSingleton<IUIExtension>(new UIExtension("OAuthPluginHeaderNav", "header-nav"));
        services.AddHostedService<ApplicationPartsLogger>();
        services.AddHostedService<PluginMigrationRunner>();
        services.AddSingleton<OAuthService>();
        services.AddSingleton<OAuthDbContextFactory>();
        services.AddDbContext<OAuthPluginDbContext>((provider, o) =>
        {
            OAuthDbContextFactory factory = provider.GetRequiredService<OAuthDbContextFactory>();
            factory.ConfigureBuilder(o);
        });
                    services.AddAuthentication()
            .AddScheme<OAuthAuthenticationOptions, OAuthAPIAuthenticationHandler>("OAuth.API", options => { });
            services.Configure<MvcOptions>(options =>
            {
                options.Conventions.Add(new OAuthControllerConvention());
                options.Conventions.Add(new OAuthActionConvention());
            });
    }
}
