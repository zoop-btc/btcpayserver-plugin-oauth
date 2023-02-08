using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BTCPayServer.Plugins.OAuth.Services;
using Microsoft.AspNetCore.Identity;
using BTCPayServer.Data;
using BTCPayServer.Plugins.OAuth.Data.Models;
using BTCPayServer.Client;
using System.Linq;
using PasswordGenerator;
using Newtonsoft.Json;

namespace BTCPayServer.Plugins.OAuth.Auth
{
    public class OAuthAuthenticationOptions : AuthenticationSchemeOptions
    {
    }
    public class OAuthAPIAuthenticationHandler : AuthenticationHandler<OAuthAuthenticationOptions>
    {
        private readonly OAuthService _oAuthService;
        private readonly ILogger _logger;
        private readonly IOptionsMonitor<IdentityOptions> _identityOptions;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;


        public OAuthAPIAuthenticationHandler(
            IOptionsMonitor<IdentityOptions> identityOptions,
            IOptionsMonitor<OAuthAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            OAuthService oAuthService
        )
            : base(options, logger, encoder, clock)
        {
            _oAuthService = oAuthService;
            _identityOptions = identityOptions;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger.CreateLogger<OAuthAPIAuthenticationHandler>();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // if (!_oAuthService.GetSettings().KratosEnabled)
            //     return AuthenticateResult.NoResult();
            try
            {
                // Check if Header is set
                if (Request.Headers.ContainsKey("Authorization"))
                {
                    var token = Request.Headers["Authorization"].ToString();

                    if (!token.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase))
                        return AuthenticateResult.NoResult();
                    var session = await _oAuthService.GetSessionByToken(token.Substring("Bearer ".Length));

                    var sessionvalid = _oAuthService.checkSessionValidity(session);

                    Console.WriteLine(sessionvalid.Message);

                    if(!sessionvalid.Valid){
                        return AuthenticateResult.Fail(sessionvalid.Message);
                    }

                    // var jsonString = JsonConvert.SerializeObject(session, Formatting.Indented);

                    // Console.WriteLine($"Received request for session:\n {jsonString}");

                    return await AuthenticateOAuthUser(session);
                }

                // If neither Cookie nor Authorization header was set, the request can't be authenticated.
                return AuthenticateResult.NoResult();
            }
            catch (Exception ex)
            {
                // If an error occurs while trying to validate the token, the Authentication request fails.
                return AuthenticateResult.Fail(ex.Message);
            }
        }

            private async Task<AuthenticateResult> AuthenticateOAuthUser(OAuthSession session)
            {
                //We get the user if he already exists
                var user = (await _userManager.FindByIdAsync(session.Subject)) ?? await _userManager.FindByEmailAsync(session.Email);

                //Should user not exist we create a new one
                if (user is null)
                {
                    var newuser = new ApplicationUser
                    {
                        UserName = session.Email,
                        Email = session.Email,
                        RequiresEmailConfirmation = false,
                        Created = DateTimeOffset.UtcNow
                    };

                    //We use the GUID from Kratos here. That way we can correlate the users even if they change the email address.
                    newuser.Id = session.Subject;

                    //Generate a random password
                    var password = new Password(includeLowercase: true, includeUppercase: true, includeNumeric: true, includeSpecial: false, passwordLength: 32).Next();

                    var result = await _userManager.CreateAsync(newuser, password);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Registered new user: {session.Email} with ID {session.Subject}");
                        user = await _userManager.FindByIdAsync(session.Subject);

                    }
                    else
                    {
                        _logger.LogWarning($"Could not create user {session.Subject} because of an error.");
                        return AuthenticateResult.NoResult();
                    }
                }

                //TODO: Finetune permissions for user
                var claims = new List<Claim>()
                {
                    new Claim(_identityOptions.CurrentValue.ClaimsIdentity.UserIdClaimType, user.Id),
                    new Claim("APIKey.Permission", Permission.Create(Policies.Unrestricted).ToString())
                };
                claims.AddRange((await _userManager.GetRolesAsync(user)).Select(s => new Claim(_identityOptions.CurrentValue.ClaimsIdentity.RoleClaimType, s)));

                return AuthenticateResult.Success(new AuthenticationTicket(
                        new ClaimsPrincipal(new ClaimsIdentity(claims, "Greenfield")), "Greenfield"));
            }
        }
    }