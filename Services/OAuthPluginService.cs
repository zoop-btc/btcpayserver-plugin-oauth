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

    // public OAuthSession Session {get;set;}
}

public class OAuthService
{
    private OAuthConf _oauthSettings;
    private readonly OAuthDbContextFactory _PluginDbContextFactory;
    private readonly ISettingsRepository _settingsRepository;
    private readonly HttpClient _client;

    public OAuthService(OAuthDbContextFactory PluginDbContextFactory, ISettingsRepository settingsRepository)
    {
        _client = new HttpClient();
        _PluginDbContextFactory = PluginDbContextFactory;
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
        await using OAuthPluginDbContext context = _PluginDbContextFactory.CreateContext();

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
        var res = await _client.PostAsync(_oauthSettings.Intro_Endpoint, content);
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
        }

        return session;
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
        //TODO: make valid scopes configurable
        if (!(session.Scope.Contains("payportal") || session.Scope.Contains("btcpay")))
            return new SessionValid(false, "token does not contain any accepted scopes");

        //TODO: saner email check with regex maybe
        if(!(session.Email.Contains("@")))
            return new SessionValid(false, $"BTCPayServer expects users to have email set. Found {session.Email} instead.");

        return new SessionValid(true, $"Token is valid until {DateTime.UnixEpoch.AddSeconds(session.ExpiresAt).ToShortDateString()} {DateTime.UnixEpoch.AddSeconds(session.ExpiresAt).ToShortTimeString()}");
    }
#nullable disable

    public async Task<List<OAuthSession>> GetSessions()
    {
        await using OAuthPluginDbContext context = _PluginDbContextFactory.CreateContext();

        return await context.OAuthSessions.ToListAsync();
    }
}

