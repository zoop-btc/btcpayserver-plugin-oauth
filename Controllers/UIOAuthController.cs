using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BTCPayServer.Abstractions.Constants;
using BTCPayServer.Abstractions.Contracts;
using BTCPayServer.Client;
using BTCPayServer.Plugins.OAuth.Data;
using BTCPayServer.Plugins.OAuth.Data.Models;
using BTCPayServer.Plugins.OAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BTCPayServer.Plugins.OAuth;

[Route("~/plugins/oauth")]
[Authorize(Policy = BTCPayServer.Client.Policies.CanModifyServerSettings,
               AuthenticationSchemes = AuthenticationSchemes.Cookie)]
public class UIOAuthController : Controller
{
    private readonly OAuthService _OAuthService;
    private readonly ISettingsRepository _settingsRepository;

    public UIOAuthController(OAuthService OAuthService, ISettingsRepository settingsRepository)
    {
        _OAuthService = OAuthService;
        _settingsRepository = settingsRepository;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        return View(new OAuthPageViewModel
        {
            Sessions = await _OAuthService.GetSessions(),
            OAuthConfig = await _settingsRepository.GetSettingAsync<OAuthConf>()
        });
    }

        [HttpPost]
        public async Task<IActionResult> Index(OAuthPageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var config = model.OAuthConfig;
                await _settingsRepository.UpdateSetting(config);
                _OAuthService.RefreshOAuthSettings();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(model);
            }
        }


}

public class OAuthPageViewModel
{
    public List<OAuthSession> Sessions { get; set; }

    [Display(Name = "OAuth Configuration")]
    public OAuthConf OAuthConfig { get; set; }
}
