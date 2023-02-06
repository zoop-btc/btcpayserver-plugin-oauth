using System.Collections.Generic;
using System.Threading.Tasks;
using BTCPayServer.Abstractions.Constants;
using BTCPayServer.Client;
using BTCPayServer.Plugins.OAuth.Data.Models;
using BTCPayServer.Plugins.OAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BTCPayServer.Plugins.OAuth;

[Route("~/plugins/template")]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.Cookie, Policy = Policies.CanViewProfile)]
public class UIPluginController : Controller
{
    private readonly OAuthService _PluginService;

    public UIPluginController(OAuthService PluginService)
    {
        _PluginService = PluginService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        return View(new PluginPageViewModel { Data = await _PluginService.Get() });
    }
}

public class PluginPageViewModel
{
    public List<OAuthSession> Data { get; set; }
}
