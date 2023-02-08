using System.Threading;
using System.Threading.Tasks;
using BTCPayServer.Abstractions.Contracts;
using BTCPayServer.Plugins.OAuth.Data;

namespace BTCPayServer.Plugins.OAuth.Services;
public class OAuthInitConfig : IStartupTask
{
    public static OAuthConf DefaultOAuthConf()
    {
        return new OAuthConf
        {
            Intro_Endpoint = "https://admin.hydra.testnet.brondings.com/admin/oauth2/introspect"
        };
    }
    private readonly ISettingsRepository _settingsRepository;
    public OAuthInitConfig(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var OAuthConfig = await _settingsRepository.GetSettingAsync<OAuthConf>();

        if (OAuthConfig == null)
        {
            await _settingsRepository.UpdateSetting<OAuthConf>(DefaultOAuthConf());
        }
    }

}