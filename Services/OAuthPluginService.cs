using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BTCPayServer.Plugins.OAuth.Data.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
    private readonly OAuthDbContextFactory _PluginDbContextFactory;

    private readonly HttpClient _client;

    public OAuthService(OAuthDbContextFactory PluginDbContextFactory)
    {
        _client = new HttpClient();
        _PluginDbContextFactory = PluginDbContextFactory;
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
            return session;
        }

        //Fetch it from hydra
        var data = new[] { new KeyValuePair<string, string>("token", token) };
        var content = new FormUrlEncodedContent(data);
        var res = await _client.PostAsync("https://admin.hydra.testnet.brondings.com/admin/oauth2/introspect", content);
        res.EnsureSuccessStatusCode();
        var json = await res.Content.ReadAsStringAsync();

        session = System.Text.Json.JsonSerializer.Deserialize<OAuthSession>(json);
        session.Token = token;
        session.Extra.Id = token;


        //If the session is valid we persist it
        var validator = checkSessionValidity(session);
        if (validator.Valid)
        {
            Console.WriteLine("Persisting new session into database");
            var result = context.OAuthSessions.Add(session);
            try{
            context.SaveChanges();
            }catch(Exception e){
                Console.WriteLine(e);
            }
            Console.WriteLine($"DB result is {result}");
        }

        return session;
    }

#nullable enable
    //TODO: I should be able to do this more elegantly
    public SessionValid checkSessionValidity(OAuthSession? session)
    {
        // var sessionvalid = new SessionValid();
        //Check for null
        if (session is null)
            return new SessionValid(false, "could not extract session from token, it is null");
        //Check if it's active
        if (!session.Active)
            return new SessionValid(false, "token is marked as inactive by OAuth provider");
        //Check if it's expired
        var unix_time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (session.ExpiresAt < unix_time)
            return new SessionValid(false, "token expired");
        //Check if it's only valid in the future
        if (session.NotBefore > unix_time)
            return new SessionValid(false, "token not yet valid");
        //Check Scopes
        if (!(session.Scope.Contains("payportal") || session.Scope.Contains("btcpay")))
            return new SessionValid(false, "token does not contain any accepted scopes");
        //Check issuer
        // var disco = await _client.GetDiscoveryDocumentAsync(session.I);
        //TODO Settings
        if (session.Issuer != "https://hydra.testnet.brondings.com/")
            return new SessionValid(false, "token is not from configured issuer");

        //TODO check that email field is populated

        return new SessionValid(true, $"token is valid until {DateTime.UnixEpoch.AddSeconds(session.ExpiresAt).ToShortDateString()} {DateTime.UnixEpoch.AddSeconds(session.ExpiresAt).ToShortTimeString()}");
    }
#nullable disable
    // public async Task AddTestDataRecord()
    // {
    //     await using OAuthPluginDbContext context = _PluginDbContextFactory.CreateContext();

    //     await context.PluginRecords.AddAsync(new OauthPluginData { Timestamp = DateTimeOffset.UtcNow });
    //     await context.SaveChangesAsync();
    // }

    public async Task<List<OAuthSession>> Get()
    {
        await using OAuthPluginDbContext context = _PluginDbContextFactory.CreateContext();

        return await context.OAuthSessions.ToListAsync();
    }
}

