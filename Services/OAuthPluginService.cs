using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using BTCPayServer.Plugins.OAuth.Data.Models;
using BTCPayServer.Plugins.OAuth.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BTCPayServer.Abstractions.Contracts;

namespace BTCPayServer.Plugins.OAuth.Services;

public class SessionValid
{
    public bool Valid { get; set; }
    public string Message { get; set; }

    public SessionValid(bool valid, string message)
    {
        Valid = valid;
        Message = message;
    }
}

public class OAuthService
{
    private OAuthConf _oauthSettings;
    private readonly OAuthDbContextFactory _OAuthDbContextFactory;
    private readonly ISettingsRepository _settingsRepository;
    private readonly HttpClient _client;

    public OAuthService(OAuthDbContextFactory OAuthDbContextFactory, ISettingsRepository settingsRepository)
    {
        _client = new HttpClient();
        _OAuthDbContextFactory = OAuthDbContextFactory;
        _settingsRepository = settingsRepository;
        RefreshOAuthSettings();
    }
    public void RefreshOAuthSettings()
    {
        _oauthSettings = _settingsRepository.GetSettingAsync<OAuthConf>().Result;
    }

    public OAuthConf GetSettings()
    {
        return _oauthSettings;
    }
    public async Task<OAuthSession> GetSessionByToken(string token)
    {
        Console.WriteLine($" Received token for introspection: {token}");
        await using OAuthPluginDbContext context = _OAuthDbContextFactory.CreateContext();

        //Check for database entry
        Console.WriteLine("Looking up session in database");
        var session = await context.OAuthSessions.FindAsync(token);

        if (session is not null)
        {
            Console.WriteLine($"Session for token {token} has been found in Database:");
            Console.WriteLine(JsonConvert.SerializeObject(session, Formatting.Indented));
            // Console.WriteLine($"Email is: {session.Extra.Email}");
            return session;
        }

        //Fetch it from hydra
        var data = new[] { new KeyValuePair<string, string>("token", token) };
        var content = new FormUrlEncodedContent(data);
        var res = await _client.PostAsync(_oauthSettings.IntroEndpoint, content);
        res.EnsureSuccessStatusCode();
        var json = await res.Content.ReadAsStringAsync();

        //Build the session object by parsing the json
        session = System.Text.Json.JsonSerializer.Deserialize<OAuthSession>(json);
        var sessionjson = JObject.Parse(json);
        session.Token = token;
        session.Email = (string)sessionjson?["ext"]?["email"] ?? "";
        session.Identifier = (string)sessionjson?["ext"]?["identifier"] ?? "";
        JArray audience = (JArray)sessionjson["aud"];
        session.Audience = String.Join(" ", audience.Select(o => o.ToString()));

        //If the session is valid we persist it
        var validator = checkSessionValidity(session);
        if (validator.Valid)
        {
            var result = context.OAuthSessions.Add(session);
            context.SaveChanges();
            return session;
        }
        else
        {
            //We can throw here because this is only run from AuthenticationHandler
            throw new Exception(validator.Message);
        }


    }

#nullable enable
    //TODO: I should be able to do this more elegantly
    public SessionValid checkSessionValidity(OAuthSession? session)
    {
        //Check for null
        if (session is null)
            return new SessionValid(false, "could not extract session from token, it is null");

        //Check if it's active
        if (!session.Active)
            return new SessionValid(false, "token is marked as inactive by OAuth provider");

        //Check Scopes
        var validscopes = _oauthSettings.AllowedScopes.Split(';');
        var tokenscopes = session.Scope.Split(' ');
        var scopes = tokenscopes.Intersect(validscopes, StringComparer.OrdinalIgnoreCase);

        // if (!(session.Scope.Contains("payportal") || session.Scope.Contains("btcpay")))
        if (!scopes.Any())
            return new SessionValid(false, $"token does not contain any accepted scopes, please make sure it has any of '{_oauthSettings.AllowedScopes}'");

        //TODO: saner email check with regex maybe
        if (!(session.Email.Contains("@")))
            return new SessionValid(false, $"BTCPayServer expects users to have email set. Found {session.Email} instead.");

        if (session.ExpiresAt < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            return new SessionValid(false, "Token expired");

        return new SessionValid(true, $"Token is valid until {DateTime.UnixEpoch.AddSeconds(session.ExpiresAt).ToShortDateString()} {DateTime.UnixEpoch.AddSeconds(session.ExpiresAt).ToShortTimeString()}");
    }
#nullable disable

    public async Task<List<OAuthSession>> GetSessions()
    {
        await using OAuthPluginDbContext context = _OAuthDbContextFactory.CreateContext();

        return await context.OAuthSessions.ToListAsync();
    }
}

